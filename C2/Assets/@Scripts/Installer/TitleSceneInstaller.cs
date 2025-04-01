using UnityEngine;
using Zenject;
using C2Project.Signals;


public class TitleSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("TitleSceneInstaller Init");

        // View
        Container.BindInterfacesAndSelfTo<TitleSceneService>().AsSingle();
        Container.Bind<UI_TitleScene>().FromComponentInHierarchy().AsSingle();


        Container.DeclareSignal<LoginSuccessSignal>();
    }
    
}