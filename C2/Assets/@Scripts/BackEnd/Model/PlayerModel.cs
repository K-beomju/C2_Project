using System;
using BackEnd;
using LitJson;
using UnityEngine;
using Zenject;

namespace C2Project.Model
{

    [Serializable]
    public class PlayerModel : IModel
    {
        public int gold;

        public DateTime lastLoginTime;
        public PlayerModel() => InitializeData();
        public void InitializeData()
        {
            gold = 10;
        }

        public Param GetParam()
        {
            return new Param()
            {
                { "gold", gold }
            };
        }

        public void PasteValues(JsonData json)
        {
            gold = int.Parse(json["gold"].ToString());
            Debug.Log($"[PlayerModel] PasteValues gold: {gold}");
        }
    }

}