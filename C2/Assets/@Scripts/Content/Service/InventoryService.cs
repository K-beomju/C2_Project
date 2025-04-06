using C2Project.BackEnd;
using C2Project.Model;
using C2Project.Setting;
using UnityEngine;
using Zenject;
using static Define;

namespace C2Project.Content
{
    public class InventoryService : IInitializable
    {
        [Inject] private WeaponModel _weaponModel;
        [Inject] private WeaponSetting _weaponSetting;
        [Inject] private GunSlotController _gunSlotController;
        [Inject] private BackEndTableSerivce _backEndTableSerivce;
        public void Initialize()
        {
            var gunInventory = _weaponModel.gunInventory;
            if(gunInventory == null || gunInventory.Count == 0)
            {
                Debug.Log("gunInventory is null or empty");
                // 게임 처음 접속한 거로 간주
                var gunData = _weaponSetting.gunDataList["item_revolver"];
                _weaponModel.AddGunToInventory(gunData); 
                _weaponModel.EquipGun(gunData.itemId);
                _gunSlotController.AmountGun(_weaponModel.GetEquipSlotIndex(gunData.itemId));
            }         


            _backEndTableSerivce.AddTransaction(TableNames.weapon,   _weaponModel.GetParam(WeaponParam.equippedGuns, WeaponParam.gunInventory));
                        _backEndTableSerivce.RunTransactionWrite();

        }


    }

}