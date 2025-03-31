using BackEnd;
using LitJson;
using UnityEngine;


namespace C2Project.Model
{
    public class UpgradeModel : IModel
    {
        public UpgradeModel() => InitializeData();
        public void InitializeData()
        {
           
        }


        public Param GetParam()
        {
            return new Param();
        }

        public void PasteValues(JsonData json)
        {
        
        }
    }

}