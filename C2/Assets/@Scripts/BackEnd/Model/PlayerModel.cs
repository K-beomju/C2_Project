using System;
using BackEnd;
using LitJson;


namespace C2Project.Model
{

    [Serializable]
    public class PlayerModel : IModel
    {
        public int gold;
        public int dia;

        
        public PlayerModel() => InitializeData();
        public void InitializeData()
        {
            gold = 10;
            dia = 0;
        }

        public Param ToParam()
        {
            Param param = new Param();
            param.Add("gold", gold);
            param.Add("dia", dia);
            return param;
        }

        public void PasteValues(JsonData json)
        {
            gold = int.Parse(json["gold"].ToString());
            dia = int.Parse(json["dia"].ToString());
        }
    }

}