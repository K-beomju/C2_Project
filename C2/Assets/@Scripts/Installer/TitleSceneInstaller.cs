using UnityEngine;
using Zenject;

public class TitleSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Hierarchy 
        Container.Bind<TitleScene>().FromComponentInHierarchy().AsSingle();
        Container.Bind<UI_TitleScene>().FromComponentInHierarchy().AsSingle();
    }
    
}