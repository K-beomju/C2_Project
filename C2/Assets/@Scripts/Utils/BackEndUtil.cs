using System;
using System.Collections.Generic;
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
                    else if (curFieldType.IsGenericType && curFieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        var elementType = curFieldType.GetGenericArguments()[0]; // List<T>의 T 타입
                        var listInstance = Activator.CreateInstance(curFieldType) as System.Collections.IList;

                        if (jsonData[fieldName].IsArray)
                        {
                            foreach (var item in jsonData[fieldName])
                            {
                                var element = Convert.ChangeType(item.ToString(), elementType);
                                listInstance?.Add(element);
                            }
                        }

                        curField.SetValue(model, listInstance);
                    }
                    else if(curFieldType.IsGenericType && curFieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                    {
                        var keyType = curFieldType.GetGenericArguments()[0]; // Dictionary<K,V>의 K 타입
                        var valueType = curFieldType.GetGenericArguments()[1]; // Dictionary<K,V>의 V 타입

                        if (keyType == typeof(string))
                        {
                            var dictionaryInstance = Activator.CreateInstance(curFieldType) as System.Collections.IDictionary;

                            if (jsonData[fieldName].IsObject)
                            {
                                foreach (var key in jsonData[fieldName].Keys)
                                {
                                    var valueJson = jsonData[fieldName][key];
                                    var value = Activator.CreateInstance(valueType);

                                    // 재귀적으로 SyncWithModel 호출
                                    SyncWithModel(value, valueJson);

                                    dictionaryInstance?.Add(key, value);
                                }
                            }

                            curField.SetValue(model, dictionaryInstance);
                        }
                    }
                    else if (curFieldType.IsArray)
                    {
                        var elementType = curFieldType.GetElementType(); // 배열의 요소 타입 (T)
                        if (jsonData[fieldName].IsArray)
                        {
                            var arrayLength = jsonData[fieldName].Count; // JSON 배열의 길이
                            var arrayInstance = Array.CreateInstance(elementType, arrayLength); // 배열 생성

                            for (int i = 0; i < arrayLength; i++)
                            {
                                var item = jsonData[fieldName][i];
                                var element = Convert.ChangeType(item.ToString(), elementType); // 요소 변환
                                arrayInstance.SetValue(element, i); // 배열에 요소 추가
                            }

                            curField.SetValue(model, arrayInstance); // 모델 필드에 배열 설정
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