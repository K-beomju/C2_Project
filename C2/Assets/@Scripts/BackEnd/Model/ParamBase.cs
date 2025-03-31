using BackEnd;
using LitJson;
using UnityEngine;

public interface IModel
{
    Param GetParam();

    void InitializeData();

    void PasteValues(JsonData json);
}
