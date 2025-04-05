using System;
using System.Reflection;
using LitJson;
using UnityEngine;

namespace C2Project.Utils
{
    public static class BackEndUtil
    {
        public static void SyncWithModel<T>(T model, JsonData jsonData)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var curField in fields)
            {
                var fieldName = curField.Name;
                var curFieldType = curField.FieldType;

                // JSON 데이터에 해당 필드가 없으면 건너뜀
                if (!jsonData.ContainsKey(fieldName))
                    continue;

                try
                {
                    if (curFieldType == typeof(int))
                    {
                        curField.SetValue(model, int.Parse(jsonData[fieldName].ToString()));
                    }
                    else if (curFieldType == typeof(double))
                    {
                        curField.SetValue(model, double.Parse(jsonData[fieldName].ToString()));
                    }
                    else if (curFieldType == typeof(float))
                    {
                        curField.SetValue(model, float.Parse(jsonData[fieldName].ToString()));
                    }
                    else if (curFieldType == typeof(string))
                    {
                        curField.SetValue(model, jsonData[fieldName].ToString());
                    }
                    else if (curFieldType == typeof(bool))
                    {
                        curField.SetValue(model, bool.Parse(jsonData[fieldName].ToString()));
                    }
                    else if (curFieldType == typeof(DateTime))
                    {
                        if (DateTime.TryParse(jsonData[fieldName].ToString(), out var dateTime))
                        {
                            curField.SetValue(model, dateTime);
                        }
                        else
                        {
                            curField.SetValue(model, default(DateTime));
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"[SyncWithModel] Unsupported field type: {curFieldType.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[SyncWithModel] Error syncing field '{fieldName}': {ex.Message}");
                }
            }
        }
    }
}