using System;
using System.Collections.Generic;
using BackEnd;
using C2Project.Model;
using C2Project.Setting;
using UnityEngine;
using Zenject;
using static Define;

namespace C2Project.Model
{

    public static class WeaponParam
    {
        public const string equippedGuns = "equippedGuns";

        public const string gunInventory = "gunInventory";
    }


    public partial class WeaponModel
    {
        public string[] equippedGuns { get; private set; }
        public Dictionary<string, GunData> gunInventory { get; private set; }
    }


    public partial class WeaponModel : ParamBase
    {

        public WeaponModel()
        {
            gunInventory = new Dictionary<string, GunData>();
            equippedGuns = new string[5];
        }

        public override Param GetParam()
        {
            Param param = new Param()
            {
                { WeaponParam.equippedGuns, equippedGuns },
                { WeaponParam.gunInventory, gunInventory }
            };
            return param;
        }

        public void EquipGun(string itemId)
        {
            if (gunInventory.TryGetValue(itemId, out GunData gunData))
            {
                if (Array.Exists(equippedGuns, g => g == gunData.itemId))
                {
                    Debug.LogWarning("이미 장착된 총기입니다.");
                    return;
                }

                int equipIndex = Array.FindIndex(equippedGuns, g => g == null);
                if (equipIndex == -1)
                {
                    Debug.LogWarning("장착 슬롯이 모두 차있습니다.");
                    return;
                }

                // 장착 슬롯에 총기 장착
                equippedGuns[equipIndex] = gunData.itemId;
                Debug.Log($"장착 슬롯 : {equipIndex + 1} 장착 총기 : {gunData.itemId}");
            }
            else
            {
                Debug.LogWarning("인벤토리에 존재하지 않는 총기입니다.");
            }
        }

        public void AddGunToInventory(GunData gunData)
        {
            if (gunInventory.ContainsKey(gunData.itemId))
            {
                Debug.LogWarning("이미 인벤토리에 존재하는 총기입니다.");
                return;
            }

            gunInventory.Add(gunData.itemId, gunData);
            Debug.Log($"총기 추가: {gunData.itemId}");
        }

        public (string, int) GetEquipSlotIndex(string itemId)
        {
            int index = Array.FindIndex(equippedGuns, g => g == itemId);
            if (index != -1)
            {
                return (itemId, index);
            }
            else
            {
                return (itemId, -1);
            }
        }
    }

}
