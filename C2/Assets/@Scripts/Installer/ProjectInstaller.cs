using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Core;
using C2Project.Model;
using C2Project.Setting;
using C2Project.Signals;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("ProjectInstaller Init");
        // GameObject
        Container.Bind<CoroutineHandler>().FromNewComponentOnNewGameObject().AsSingle();

        // Models
        Container.Bind<PlayerModel>().AsSingle();
        Container.Bind<UpgradeModel>().AsSingle();
        Container.Bind<WeaponModel>().AsSingle();

        // Settings
        Container.Bind<WeaponSetting>().AsSingle();

        // Services
        Container.BindInterfacesAndSelfTo<AddressableService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PopupService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SceneService>().AsSingle();
        Container.BindInterfacesAndSelfTo<SpriteLoader>().AsSingle();
        Container.BindInterfacesAndSelfTo<ResourceLoader>().AsSingle();

        // BackEnd
        Container.BindInterfacesAndSelfTo<BackEndAuthService>().AsSingle();
        Container.BindInterfacesAndSelfTo<BackEndTableSerivce>().AsSingle();
        Container.BindInterfacesAndSelfTo<BackEndUtilService>().AsSingle();
        Container.BindInterfacesAndSelfTo<BackEndChartService>().AsSingle();


        SignalBusInstaller.Install(Container);
    }
}