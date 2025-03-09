using UnityEngine;
using Zenject;

public class TitleScene : MonoBehaviour
{
    [Inject]
    private BackEndAuthService _backEndAuthService;
    [Inject]
    private AddressableService _addressableService;

    private void Awake()
    {
        _backEndAuthService.Init();
    }

    private async void Start()
    {
        await _addressableService.LoadAllPrefabsAsync("Load", (key, count, totalCount) => 
        {
            Debug.Log($"[GameManager] Loaded: {key} ({count}/{totalCount})");

            if (count == totalCount)
            {
                Debug.Log("[GameManager] All Prefabs Loaded.");
            }
        });
    }

    private void StartProgress()
    {
        
    }

}
