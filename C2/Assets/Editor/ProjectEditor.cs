using BackEnd;
using UnityEditor;
using UnityEngine;

public class ProjectEditor
{
    [MenuItem("Tools/ClearPlayerPrefs")]
    public static void ClearPlayerPrefs()
    {
        SecurityPlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/ClearGuestInfo")]
    public static void ClearGuestInfo()
    {
        Backend.BMember.DeleteGuestInfo();
    }
}
