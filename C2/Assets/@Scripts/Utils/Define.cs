using UnityEngine;

public static class Define
{
    // 뒤끝 서버 상태
    public enum EServerStatus
    {
        None,
        Online,
        Offline,
        Maintenance,
    }

    // 타이틀씬 로그인 타입 
    public enum ELoginType 
    {
        None,
        Google,
        Apple,
        Guest
    }

    public static class TableNames 
    {
        public const string player = "player";
        
        public const string upgrade = "upgrade"; 
    }
}
