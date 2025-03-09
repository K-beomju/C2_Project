using UnityEngine;
using BackEnd;
using static Define;
using System;

public class BackEndAuthService : IInitailizeable
{   
    protected bool _init = false;

    public bool Init()
    {
        if (_init)
            return false;

        var bro = Backend.Initialize();

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
        }
        _init = true;
        return true;
    }

    public void AuthLogin(ELoginType loginType)
    {
        switch (loginType)
        {
            case ELoginType.Google:
                GoogleLogin();
                break;
            case ELoginType.Apple:
                AppleLogin();
                break;
            case ELoginType.Guest:
                GuestLogin();
                break;
        }
    }

    private void GoogleLogin()
    {
    }

    private void AppleLogin()
    {
    }

    private void GuestLogin()
    {
        SendQueue.Enqueue(Backend.BMember.GuestLogin, (callback) =>
        {
            if(Backend.BMember.GetGuestID() != null)
            {
                string id = Backend.BMember.GetGuestID();
                Debug.Log("로컬 기기에 저장된 아이디로 게스트 로그인 합니다. :" + id);
            }

            if (callback.IsSuccess())
            {
                Debug.Log("게스트 로그인 성공 : " + callback); // 성공일 경우 statusCode 200 Success
            }
            else
            {
                Debug.LogError("게스트 로그인 실패 : " + callback); // 실패일 경우 statusCode 400대 에러 발생
            }
        });
    }

}
