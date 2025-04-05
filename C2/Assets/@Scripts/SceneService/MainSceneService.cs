using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Model;
using UnityEngine;
using Zenject;
using static Define;
public class MainSceneService : IInitializable
{
    [Inject] private BackEndTableSerivce _backEndTableSerivce;
    [Inject] private AddressableService _addressableService;
    [Inject] private PlayerModel playerModel;
    public void Initialize()
    {
        Debug.Log("MainSceneService Init");
        GameObject prefab = new GameObject("TestPrefab");
        prefab.transform.position = new Vector3(0, 0, 0);
        prefab.AddComponent<SpriteRenderer>().sprite = _addressableService._spriteCache["AK47"];
        
    }
}
