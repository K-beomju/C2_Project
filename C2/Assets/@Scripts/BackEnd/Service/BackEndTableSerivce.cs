using UnityEngine;
using Zenject;
using BackEnd;
using System.Collections.Generic;
using System;
using RSG;
using LitJson;
using System.Linq;
using C2Project.Model;
using static Define;

namespace C2Project.BackEnd
{
    public class BackEndTableSerivce : IInitializable, IDisposable
    {
        [Inject] private PlayerModel _playerModel;
        [Inject] private UpgradeModel _upgradeModel;

        private Dictionary<string, string> _myInDateDictionary;
        private List<string> _emptyModelNameList;
        private Dictionary<string, IModel> _modelDictionary;
        private List<TransactionValue> _transactionWriteList;

        public void Initialize()
        {
            _emptyModelNameList = new List<string>();
            _myInDateDictionary = new Dictionary<string, string>();
            _modelDictionary = new Dictionary<string, IModel>
            {
                { TableNames.player, _playerModel },
                { TableNames.upgrade , _upgradeModel }

            };

            _transactionWriteList = new List<TransactionValue>();
        }

        public void Dispose()
        {
            _myInDateDictionary.Clear();
        }

        public IEnumerable<IPromise> LoadAllTables()
        {
            _emptyModelNameList.Clear();
            _myInDateDictionary.Clear();

            return new List<IPromise>
            {
                LoadMyTableWithPromise(TableNames.player, _playerModel),
                LoadMyTableWithPromise(TableNames.upgrade, _upgradeModel),
            };
        }
        
        public IPromise LoadMyTableWithPromise(string tableName, IModel model)
        {
            var promise = new Promise();
            SendQueue.Enqueue(Backend.GameData.GetMyData, tableName, new Where(), 1, bro =>
            {
                if (bro.IsSuccess() == false)
                {
                    // 요청 실패 처리
                    promise.Reject(new Exception($"[LoadMyTableWithPromise] {tableName} Load Failed"));
                    return;
                }
                else
                {
                    if (IsEmptyTable(bro.Rows()))
                    {
                        _emptyModelNameList.Add(tableName);
                    }
                    else
                    {
                        model.PasteValues(bro.FlattenRows()[0]);
                        var tableInData = bro.GetInDate();
                        _myInDateDictionary.Add(tableName, tableInData);
                    }
                    promise.Resolve();
                }
            });

            return promise;
        }

        public void AddTransaction(string tableName)
        {   
            _transactionWriteList.Add(TransactionValue.SetUpdateV2(tableName, _myInDateDictionary[tableName], Backend.UserInDate, _modelDictionary[tableName].GetParam()));
        }

        public IPromise RunTransactionWrite()
        {
            var promise = new Promise();

            if(_transactionWriteList.Count == 0)
            {
                Debug.Log($"[TransactionUpdate] No Transaction");
                promise.Resolve();
                return promise;
            }

            SendQueue.Enqueue(Backend.GameData.TransactionWriteV2, _transactionWriteList, bro =>
            {
                var tableNameList = _transactionWriteList.Select(tr => tr.table).ToArray();
                var transactionList = _transactionWriteList.ToList();
                _transactionWriteList.Clear();

                if (bro.IsSuccess())
                {
                    Debug.Log(string.Join(", ", tableNameList) + " Transaction Success");
                    promise.Resolve();
                }
                else
                {
                    promise.Reject(new Exception($"[TransactionUpdate] {string.Join(", ", tableNameList)} Transaction Failed"));
                }
            });
            return promise;
        }


        private bool IsEmptyTable(JsonData rows) => rows.Count == 0;


    }

}