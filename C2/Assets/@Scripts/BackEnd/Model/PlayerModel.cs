using System;
using System.Diagnostics;
using BackEnd;
using C2Project.Utils;
using LitJson;


namespace C2Project.Model
{
    public static class PlayerParam 
    {
        public const string gold = "gold";
        public const string dia = "dia";
        public const string level = "level";
    }

    [Serializable]
    public class PlayerModel : ParamBase
    {
        public int gold;
        public int dia;

        public int level;
        
        public PlayerModel()
        {
            gold = 10;
            dia = 0;
            level = 1;
        } 
        public override Param GetParam()
        {
            Param param = new Param
            {
                { PlayerParam.gold, gold },
                { PlayerParam.dia, dia },
                { PlayerParam.level, level }
            };
            return param;
        }

    }

}