using C2Project.Content;
using UnityEngine;
using Zenject;

public class MainSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("MainSceneInstaller Init");

        // Controller
        Container.Bind<GunSlotController>().FromComponentInHierarchy().AsSingle();

        // View
        Container.Bind<UI_MainScene>().FromComponentInHierarchy().AsSingle();

        // Services
        Container.BindInterfacesAndSelfTo<MainSceneService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle();

    }
}
