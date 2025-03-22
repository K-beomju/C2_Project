using System.Collections;
using BackEnd;
using C2Project.Addressable;
using C2Project.BackEnd;
using UnityEngine;
using Zenject;

public class TitleScene : InitBase
{
    [Inject] private BackEndAuthService _backEndAuthService;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;


        return true;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        
        _backEndAuthService.InitBackend()
           .Then((success) =>
           {
               Debug.Log("뒤끝 초기화 성공");
           });
    }
}
