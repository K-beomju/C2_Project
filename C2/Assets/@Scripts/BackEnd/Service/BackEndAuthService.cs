using UnityEngine;
using BackEnd;
using System;
using Zenject;
using RSG;
using C2Project.Signals;
using static Define;
using C2Project.Core;
using System.Collections;

namespace C2Project.BackEnd
{
    public class BackEndAuthService : IInitializable
    {
        [Inject] private CoroutineHandler coroutine;
        public ELoginType CurLoginType { get; private set; } = ELoginType.None;


        public void Initialize()
        {
            InitBackend().Catch(ex =>
            {
                Debug.LogError($"[BackEndAuthService] 초기화 실패: {ex.Message}");
            });
        }
        

        public IPromise<bool> InitBackend()
        {
            var promise = new Promise<bool>();

            try
            {
                var bro = Backend.Initialize();

                if (bro.IsSuccess())
                {
                    Debug.Log($"[BackEndAuthService] 초기화 성공 : " + bro);
                    promise.Resolve(true);
                }
                else
                {
                    throw new Exception("bro 초기화 실패 : " + bro);
                }
            }
            catch (Exception ex)
            {
                promise.Reject(ex); // 예외 발생 시 Reject
            }


            return promise;
        }


        // 1. 마지막 로그인 타입만 가져오기
        public ELoginType LoadLastLoginType()
        {
            string lastedLoginType = SecurityPlayerPrefs.GetString("CurLoginType", null);

            if (!string.IsNullOrEmpty(lastedLoginType))
            {
                try
                {
                    return Util.ParseEnum<ELoginType>(lastedLoginType);
                }
                catch (ArgumentException ex)
                {
                    Debug.LogError($"[BackEndAuthService] 저장된 로그인 타입 파싱 실패: {ex.Message}");
                }
            }

            return ELoginType.None;
        }


        // 2. 자동 로그인 시도
        public IPromise<ELoginType> TryAutoLogin()
        {
            var promise = new Promise<ELoginType>();
            var loginType = LoadLastLoginType();

            if (loginType == ELoginType.None)
            {
                Debug.Log("로컬에 로그인 한 전적이 없음");
                promise.Resolve(loginType);
            }
            else
            {
                AuthLogin(loginType)
                    .Then(_ =>
                    {
                        Debug.Log($"[BackEndAuthService] 자동 로그인 성공: {loginType}");
                        promise.Resolve(loginType);
                    })
                    .Catch(ex =>
                    {
                        Debug.LogError($"[BackEndAuthService] 자동 로그인 실패: {ex.Message}");
                        promise.Reject(ex);
                    });
            }

            return promise;
        }


        #region Federation Auth Login

        public IPromise<bool> AuthLogin(ELoginType loginType)
        {
            return loginType switch
            {
                ELoginType.Google => GoogleLogin(),
                ELoginType.Apple => AppleLogin(),
                ELoginType.Guest => GuestLogin(),
                _ => Promise<bool>.Rejected(new Exception("지원되지 않는 로그인 타입"))
            };
        }

        private IPromise<bool> GoogleLogin()
        {
            return Promise<bool>.Rejected(new NotImplementedException("GoogleLogin 미구현"));
        }

        private IPromise<bool> AppleLogin()
        {
            return Promise<bool>.Rejected(new NotImplementedException("AppleLogin 미구현"));
        }

        private IPromise<bool> GuestLogin()
        {
            var promise = new Promise<bool>();

            try
            {
                SendQueue.Enqueue(Backend.BMember.GuestLogin, (callback) =>
                {
                    string guestId = Backend.BMember.GetGuestID();

                    if (callback.IsSuccess())
                    {
                        CurLoginType = ELoginType.Guest;
                        SecurityPlayerPrefs.SetString("CurLoginType", CurLoginType.ToString());
                        Debug.Log($"[BackEndAuthService] 게스트 로그인 성공: {callback}");
                        promise.Resolve(true);
                    }
                    else
                    {
                        Debug.LogError($"[BackEndAuthService] 게스트 로그인 실패: {callback}");
                        promise.Reject(new Exception("게스트 로그인 실패 : " + callback));
                    }
                });
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GuestLogin] 예외 발생: {ex.Message}");
                promise.Reject(ex); // 예외 발생 시 Reject
            }

            return promise;
        }
    }

    #endregion
}