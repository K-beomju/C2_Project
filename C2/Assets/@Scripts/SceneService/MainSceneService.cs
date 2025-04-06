using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Core;
using C2Project.Model;
using UnityEngine;
using Zenject;
using static Define;
public class MainSceneService : IInitializable
{
    [Inject] private ResourceLoader resourceLoader;
    public void Initialize()
    {        
        Debug.Log("MainSceneService Init");

    }
}
