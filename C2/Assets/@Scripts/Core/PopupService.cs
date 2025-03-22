using System.Collections.Generic;
using C2Project.Addressable;
using UnityEngine;
using Zenject;

public class PopupService : IInitializable
{
    [Inject] private AddressableService _addressableService;

    private UI_Base _sceneUI = null;
    public UI_Base SceneUI
    {
        get { return _sceneUI; }
        set { _sceneUI = value; }
    }

    public void Initialize()
    {
        
    }

    public void SetCurrentSceneUI(UI_Base sceneUI)
    {
        SceneUI = sceneUI;
    }

}
