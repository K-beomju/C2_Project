using System.Collections;
using BackEnd;
using BackEnd.Functions;
using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Signals;
using UnityEngine;
using Zenject;
using static Define;

public class TitleScene : InitBase
{
    [Inject] private BackEndAuthService _backEndAuthService;
    [Inject] private BackEndTableSerivce _backEndTableSerivce;
    [Inject] private BackEndUtilService _backEndUtilService;

    [Inject] private AddressableService _addressableService;


    [Inject] private PopupService _popupService;

    [Inject] private SignalBus _signalBus;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _signalBus.Subscribe<LoginSuccessSignal>(OnLoginSuccess);
        return true;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        if (!Backend.IsInitialized)
        {
            Debug.Log("뒤끝 SDK가 초기화 되지 않았음");
            yield break;
        }

        // if (_backEndUtilService.GetServerStatus() != EServerStatus.Online)
        // {
        //     Debug.Log("서버 점검중");
        //     yield break;
        // }

        var lastLoginType = _backEndAuthService.LoadLastLoginType();

        if (lastLoginType == ELoginType.None)
        {
            Debug.Log("[TitleScene] 저장된 로그인 기록 없음");
            (_popupService.SceneUI as UI_TitleScene)?.ActiveLoginButtonGroup(true);
            yield break;
        }

        _backEndAuthService.AuthLogin(lastLoginType)
       .Then((bro) =>
       {
           var titleUI = _popupService.SceneUI as UI_TitleScene;
           if (bro)
           {
               Debug.Log($"[TitleScene] 자동 로그인 성공: {lastLoginType}");
               _signalBus.Fire(new LoginSuccessSignal());
           }
           else
           {
               Debug.LogError($"[TitleScene] 자동 로그인 실패");
           }
           titleUI?.ActiveLoginButtonGroup(!bro);

       });
    }

    private void OnLoginSuccess(LoginSuccessSignal signal)
    {
        _addressableService.LoadAllPrefabsAsync("Load")
        .Then((success) =>
        {
            if (success)
            {
                Debug.Log("Addressable Prefabs 로드 성공");
                _backEndTableSerivce.LoadAllTables();
            }
            else
            {
                Debug.LogError("Addressable Prefabs 로드 실패");
            }
        });
    }

}
