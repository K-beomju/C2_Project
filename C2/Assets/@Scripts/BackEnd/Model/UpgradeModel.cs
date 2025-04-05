using BackEnd;
using LitJson;
using UnityEngine;


namespace C2Project.Model
{
    public class UpgradeModel : IModel
    {
        public int atkLevel;
        public UpgradeModel() => InitializeData();
        public void InitializeData()
        {
            atkLevel = 1;
        }

        public Param ToParam()
        {
            return new Param()
            {
                { "atkLevel", atkLevel }
            };
        }

        public void PasteValues(JsonData json)
        {
            atkLevel = int.Parse(json["atkLevel"].ToString());
        }
    }

}