using UnityEngine;
using BackEnd;
using System;
using Zenject;
using RSG;
using C2Project.Signals;
using static Define;

namespace C2Project.BackEnd
{
    public class BackEndAuthService : IInitializable
    {
        protected bool _init = false;

        public ELoginType CurLoginType { get; private set; }


        public void Initialize()
        {
            if (_init)
                return;

            _init = true;
        }

        public IPromise<bool> InitBackend()
        {
            var promise = new Promise<bool>();

            var bro = Backend.Initialize();
            // 뒤끝 초기화에 대한 응답값

            try
            {
                if (bro.IsSuccess())
                {
                    Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
                    LoadLastLoginType();
                    promise.Resolve(true);
                    // 1. 먼저 구글/애플 로그인 이력이 있는지 확인 -> 자동 로그인
                    // 2. 그 다음 게스트 로그인 이력이 있는지 확인 -> 자동 로그인 
                }
                else
                {
                    promise.Resolve(false);
                    promise.Reject(new Exception("bro 초기화 실패 : " + bro)); // 실패일 경우 statusCode 400대 에러 발생));
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[뒤끝 SDK 초기화] 예외 발생: {ex.Message}");
                promise.Reject(ex); // 예외 발생 시 Reject
            }
            return promise;
        }

        private void LoadLastLoginType()
        {
            CurLoginType = ELoginType.None; // 기본값 설정
            string lastedLoginType = SecurityPlayerPrefs.GetString("CurLoginType", null);
            if(!string.IsNullOrEmpty(lastedLoginType))
            {
                try 
                {
                    CurLoginType = Util.ParseEnum<ELoginType>(lastedLoginType);
                }
                catch (ArgumentException ex)
                {
                    Debug.LogError($"[BackEndAuthService] 저장된 로그인 타입을 파싱하는 중 오류 발생: {ex.Message}");
                    CurLoginType = ELoginType.None; // 파싱 실패 시 기본값 설정
                }
            }
            
            if(CurLoginType == ELoginType.None)
            {
                Debug.Log("로컬에 로그인 한 전적이 없음");
            }
            else
            {
                // 여기서 자동 로그인? 
                Debug.Log($"{CurLoginType} 마지막으로 로그인 한 타입");
            }
        }

        public IPromise<bool> AuthLogin(ELoginType loginType)
        {
            var promise = new Promise<bool>();

            switch (loginType)
            {
                case ELoginType.Google:
                    GoogleLogin();
                    break;
                case ELoginType.Apple:
                    AppleLogin();
                    break;
                case ELoginType.Guest:

                    GuestLogin()
                    .Then(result => promise.Resolve(result))
                    .Catch(ex => promise.Reject(ex));
                    break;
            }

            return promise;
        }

        private void GoogleLogin()
        {
        }

        private void AppleLogin()
        {
        }

        private IPromise<bool> GuestLogin()
        {
            var promise = new Promise<bool>();

            try
            {
                SendQueue.Enqueue(Backend.BMember.GuestLogin, (callback) =>
                {
                    string guestId = Backend.BMember.GetGuestID();

                    if (!string.IsNullOrEmpty(guestId))
                        Debug.Log($"[GuestLogin] 로컬 기기에 저장된 아이디로 게스트 로그인 시도: {guestId}");

                    if (callback.IsSuccess())
                    {
                        CurLoginType = ELoginType.Guest;
                        SecurityPlayerPrefs.SetString("CurLoginType", CurLoginType.ToString());
                        Debug.Log($"[GuestLogin] 게스트 로그인 성공: {callback}");
                        promise.Resolve(true);
                    }
                    else
                    {
                        Debug.LogError($"[GuestLogin] 게스트 로그인 실패: {callback}");
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
}