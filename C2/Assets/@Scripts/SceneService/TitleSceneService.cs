using BackEnd;
using C2Project.Addressable;
using C2Project.BackEnd;
using C2Project.Signals;
using RSG;
using UnityEngine;
using Zenject;
using UniRx;
using static Define;
using C2Project.Core;

public class TitleSceneService : IInitializable
{
    [Inject] private BackEndAuthService _backEndAuthService;
    [Inject] private BackEndTableSerivce _backEndTableSerivce;
    [Inject] private BackEndUtilService _backEndUtilService;
    [Inject] private AddressableService _addressableService;
    [Inject] private SceneService _sceneService;

    [Inject] private UI_TitleScene _titleSceneUI;
    [Inject] private SignalBus _signalBus;

    public void Initialize()
    {
        _signalBus.Subscribe<LoginSuccessSignal>(OnLoginSuccess);
        InitializeScene();
    }

    private void InitializeScene()
    {
        // UniRx의 Observable.Timer를 사용하여 1초 대기
        Observable.Timer(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                if (!Backend.IsInitialized)
                {
                    Debug.Log("뒤끝 SDK가 초기화 되지 않았음");
                    return;
                }

                var lastLoginType = _backEndAuthService.LoadLastLoginType();

                if (lastLoginType == ELoginType.None)
                {
                    Debug.Log("[TitleScene] 저장된 로그인 기록 없음");
                    _titleSceneUI?.ActiveLoginButtonGroup(true);
                    return;
                }

                // 로그인 시도
                _backEndAuthService.AuthLogin(lastLoginType)
                    .Then(bro =>
                    {
                        if (bro)
                        {
                            Debug.Log($"[TitleScene] 자동 로그인 성공: {lastLoginType}");
                            _signalBus.Fire(new LoginSuccessSignal());
                        }
                        else
                        {
                            Debug.LogError($"[TitleScene] 자동 로그인 실패");
                        }

                        _titleSceneUI?.ActiveLoginButtonGroup(!bro);
                    })
                    .Catch(ex =>
                    {
                        Debug.LogError($"[TitleScene] 로그인 중 오류 발생: {ex.Message}");
                    });
            });

    }



    private void OnLoginSuccess(LoginSuccessSignal signal)
    {
        PlayDataLoadProgress()
                .Progress(v => _titleSceneUI.BindProgress(v))
                .Done(() =>
                {
                    LoadMainScene();
                });
    }
    private IPromise PlayDataLoadProgress()
    {
        var promise = new Promise();

        _addressableService.LoadAllPrefabsAsync("Load")
           .Then(() => promise.ReportProgress(0.1f))
           .Then(() => _backEndTableSerivce.LoadAllTables())
           .Then(() => promise.ReportProgress(1f))
           .Done(() =>
           {
               promise.Resolve();
           }, ex =>
           {
               promise.Reject(ex);
           });

        return promise;
    }

    private void LoadMainScene()
    {
        Debug.Log("메인씬으로 갈 준비가 끝남");
        _sceneService.LoadScene(EScene.MainScene);
    }


}
