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
using UnityEngine.U2D;
using System.Collections.Generic;
using System;

public class TitleSceneService : IInitializable
{
    [Inject] private BackEndAuthService _backEndAuthService;
    [Inject] private BackEndTableSerivce _backEndTableSerivce;
    [Inject] private BackEndChartService _backEndChartService;
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
        Util.DelayUnit(1)
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
                    Util.DelayUnit(1)
                        .Subscribe(_ =>
                        {
                            Debug.Log("로딩 완료");
                            LoadMainScene();
                        });
                });
    }

    private List<Func<IPromise>> DefineTasks()
    {
        return new List<Func<IPromise>>
        {
            () => _addressableService.LoadAllPrefabsAsync<SpriteAtlas>("Load"),
            () => _addressableService.LoadAllPrefabsAsync<GameObject>("Load"),
            () => Promise.All(_backEndChartService.LoadAllCharts()),
            () => Promise.All(_backEndTableSerivce.LoadAllTables()),
            () => Promise.All(_backEndTableSerivce.InsertTablesIfEmpty())
        };;
    }

    private IPromise PlayDataLoadProgress()
    {
        var promise = new Promise();

        // 작업 리스트 정의
        var tasks = DefineTasks();
        int totalTasks = tasks.Count;
        int completedTasks = 0;

        // 작업을 순차적으로 실행
        ExecuteTasksSequentially(tasks, progress =>
        {
            completedTasks++;
            float progressValue = (float)completedTasks / totalTasks;
            promise.ReportProgress(progressValue);
        })
        .Done(() => promise.Resolve(), ex => promise.Reject(ex));

        return promise;
    }

    private IPromise ExecuteTasksSequentially(List<Func<IPromise>> tasks, Action<float> onProgress)
    {
        var promise = new Promise();

        void ExecuteNext(int index)
        {
            if (index >= tasks.Count)
            {
                promise.Resolve();
                return;
            }

            tasks[index]()
                .Done(() =>
                {
                    onProgress?.Invoke((float)(index + 1) / tasks.Count);
                    ExecuteNext(index + 1);
                }, ex => promise.Reject(ex));
        }

        ExecuteNext(0);
        return promise;
    }

    private void LoadMainScene()
    {

        Debug.Log("메인씬으로 갈 준비가 끝남");
        _sceneService.LoadScene(EScene.MainScene);
    }


}
