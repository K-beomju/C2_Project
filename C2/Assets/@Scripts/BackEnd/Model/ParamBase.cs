using BackEnd;
using LitJson;
using UnityEngine;

public interface IModel
{
    Param ToParam();

    void InitializeData();

    void PasteValues(JsonData json);
}
