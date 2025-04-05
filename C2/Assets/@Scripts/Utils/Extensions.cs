using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void SetCanvasGroupState(this CanvasGroup canvasGroup, bool active, bool animate = false)
    {
        if (active)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    public static void AddIfEmpty<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue item)
    {
        if (dic.ContainsKey(key))
            return;

        dic.Add(key, item);
    }
}
