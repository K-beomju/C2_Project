using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Core;
using C2Project.Model;
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
        Container.Bind<SendQueueMgr>().FromNewComponentOnNewGameObject().AsSingle();


        // Model
        Container.Bind<PlayerModel>().AsSingle();
        Container.Bind<UpgradeModel>().AsSingle();

        // Services
        Container.BindInterfacesAndSelfTo<AddressableService>().AsSingle();
        Container.BindInterfacesAndSelfTo<PopupService>().AsSingle();

        // BackEnd
        Container.BindInterfacesAndSelfTo<BackEndAuthService>().AsSingle();
        Container.BindInterfacesAndSelfTo<BackEndTableSerivce>().AsSingle();
        Container.BindInterfacesAndSelfTo<BackEndUtilService>().AsSingle();
        SignalBusInstaller.Install(Container);
    }
}