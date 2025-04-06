using BackEnd;
using UnityEngine;
using BackEnd.Content;
using RSG;
using System;
using System.Collections.Generic;
using Zenject;
using C2Project.Setting;

namespace C2Project.BackEnd
{
    public class BackEndChartService
    {
        [Inject] private WeaponSetting _weaponSetting;
        
        public IEnumerable<IPromise> LoadAllCharts()
        {
            var promise = new Promise();
            var list = new List<IPromise>();

            FetchChart()
                .Then((contentDic) => 
                {   
                    list.AddRange(new List<IPromise>
                    {
                        _weaponSetting.ContentToJsonGunChart(contentDic["GunChart"].contentJson)
                    });
                    Debug.Log("LoadChart Success");
                    promise.Resolve();
                })
                .Catch(ex => Debug.LogError("Error loading charts: " + ex.Message));

            return list;
        }

        public IPromise<Dictionary<string, ContentItem>> FetchChart()
        {
            var promise = new Promise<Dictionary<string, ContentItem>>();
            Dictionary<string, ContentItem> contentDic  = new Dictionary<string, ContentItem>();

            Backend.CDN.Content.Table.Get(table =>
            {
                if (!table.IsSuccess())
                {
                    promise.Reject(new Exception("Failed to load charts"));
                    return;
                }
                
                var content = Backend.CDN.Content.Get(table.GetContentTableItemList());
                if (!content.IsSuccess())
                {
                    promise.Reject(new Exception("Failed to load content"));
                    return;
                }

                contentDic = content.GetContentDictionarySortByChartName();
                promise.Resolve(contentDic);
            });

            return promise; 
        }



    }
}
