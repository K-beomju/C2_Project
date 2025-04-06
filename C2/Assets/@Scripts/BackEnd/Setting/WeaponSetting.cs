using System;
using System.Collections.Generic;
using RSG;
using UnityEngine;
using static Define;

namespace C2Project.Setting
{
    [Serializable]
    public class GunData 
    {
        public string itemId;
        public double attackPower;
        public double attackSpeed;
        public EGradeType gradeType;

        public GunData(string itemId, double attackPower, double attackSpeed, EGradeType gradeType)
        {
            this.itemId = itemId;
            this.attackPower = attackPower;
            this.attackSpeed = attackSpeed;
            this.gradeType = gradeType;
        }
    }


    public partial class WeaponSetting
    {   
        public Dictionary<string, GunData> gunDataList = new Dictionary<string, GunData>();
    }

    public partial class WeaponSetting
    {
        public IPromise ContentToJsonGunChart(LitJson.JsonData json) 
        {
            var promise = new Promise();
            foreach(LitJson.JsonData item in json)
            {
                var itemId = item["itemId"].ToString();
                var attackPower = double.Parse(item["attackPower"].ToString());
                var attackSpeed = double.Parse(item["attackSpeed"].ToString());
                var gradeType = (EGradeType)Enum.Parse(typeof(EGradeType), item["grade"].ToString());

                if (!gunDataList.ContainsKey(itemId))
                {
                    gunDataList.Add(itemId, new GunData(itemId, attackPower, attackSpeed, gradeType));
                }
            }
            
            promise.Resolve();
            return promise;
        }
    }
}