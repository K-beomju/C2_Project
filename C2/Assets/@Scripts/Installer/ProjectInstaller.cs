using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Signals;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("ProjectInstaller Init");
        // Services
        Container.BindInterfacesAndSelfTo<AddressableService>().AsSingle();

        // BackEnd
        Container.BindInterfacesAndSelfTo<BackEndAuthService>().AsSingle();

        
        SignalBusInstaller.Install(Container);
    }
}