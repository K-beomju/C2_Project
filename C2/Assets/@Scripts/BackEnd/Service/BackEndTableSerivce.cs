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
               // { TableNames.upgrade, _upgradeModel }
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
                //LoadMyTableWithPromise(TableNames.upgrade, _upgradeModel),
            };
        }

        public IEnumerable<IPromise> InsertTablesIfEmpty()
        {
            return _emptyModelNameList
                .Select(emptyModelName => InsertMyTable(emptyModelName, _modelDictionary[emptyModelName].ToParam()))
                .ToList();
        }

        private IPromise InsertMyTable(string tableName, Param param) 
        {
            var promise = new Promise();

            SendQueue.Enqueue(Backend.GameData.Insert, tableName, param, bro =>
            {
                if (bro.IsSuccess())
                {
                    Debug.Log($"[InsertMyTable] {tableName} Insert Success");
                    _myInDateDictionary.AddIfEmpty(tableName, bro.GetReturnValuetoJSON()["inDate"].ToString());
                    promise.Resolve();
                }
                else
                {
                    promise.Reject(new Exception($"[InsertMyTable] {tableName} Insert Failed"));
                }
            });

            return promise;
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
                        Debug.Log($"[LoadMyTableWithPromise] {tableName} Load Empty");
                        _emptyModelNameList.Add(tableName);
                    }
                    else
                    {
                        Debug.Log($"[LoadMyTableWithPromise] {tableName} Load Success");
                        model.PasteValues(bro.Rows()[0]);

                        var tableInData = bro.GetInDate();
                        _myInDateDictionary.AddIfEmpty(tableName, tableInData);
                    }
                    promise.Resolve();
                }
            });

            return promise;
        }

        public void AddTransaction(string tableName)
        {   
            if (FindTableInListAndAppend(_transactionWriteList, tableName,  _modelDictionary[tableName].ToParam()) == false)
            _transactionWriteList.Add(TransactionValue.SetUpdateV2(tableName, _myInDateDictionary[tableName], Backend.UserInDate, _modelDictionary[tableName].ToParam()));
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

        private bool FindTableInListAndAppend(List<TransactionValue> transactionValues, string tableName, Param param)
        {
            var findIndex = transactionValues.FindIndex(tv => tv.table == tableName);

            if (findIndex < 0)
                return false;

            var curParam = transactionValues[findIndex].param;

            foreach (var key in param.GetKeyList())
            {
                if (curParam.ContainsKey(key))
                    curParam[key] = param[key];
                else
                    curParam.Add(key, param[key]);
            }

            return true;
        }
    }

}