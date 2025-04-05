using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("MainSceneInstaller Init");

        // View
        Container.BindInterfacesAndSelfTo<MainSceneService>().AsSingle();
        Container.Bind<UI_MainScene>().FromComponentInHierarchy().AsSingle();

    }
}
