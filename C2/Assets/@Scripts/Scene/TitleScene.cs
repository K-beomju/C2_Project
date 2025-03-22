using System.Collections;
using BackEnd;
using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Signals;
using UnityEngine;
using Zenject;
using static Define;

public class TitleScene : InitBase
{
    [Inject] private BackEndAuthService _backEndAuthService;

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


        var lastLoginType = _backEndAuthService.LoadLastLoginType();

        if (lastLoginType == ELoginType.None)
        {
            Debug.Log("[TitleScene] 저장된 로그인 기록 없음");
            (_popupService.SceneUI as UI_TitleScene)?.ActiveLoginButtonGroup(true);
            yield break;
        }

        _backEndAuthService.AuthLogin(lastLoginType)
       .Then(_ =>
       {
           Debug.Log($"[TitleScene] 자동 로그인 성공: {lastLoginType}");
           var titleUI = _popupService.SceneUI as UI_TitleScene;
           titleUI?.ActiveLoginButtonGroup(false);
           _signalBus.Fire(new LoginSuccessSignal());

       })
       .Catch(ex =>
       {
           Debug.LogError($"[TitleScene] 자동 로그인 실패: {ex.Message}");
           var titleUI = _popupService.SceneUI as UI_TitleScene;
           titleUI?.ActiveLoginButtonGroup(true); // 로그인 버튼 다시 보여주기
       });
    }

    private void OnLoginSuccess(LoginSuccessSignal signal)
    {
        var a = _addressableService.LoadAllPrefabsAsync("Load");
    }

}
