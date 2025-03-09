using UnityEngine;
using Zenject;
using static Define;

public class UI_TitleScene : UI_Base
{
    public enum Buttons 
    {
        Btn_GoogleLogin,
        Btn_AppleLogin,
        Btn_GuestLogin
    }

    [Inject] 
    private BackEndAuthService _backEndAuthService;

    public override bool Init()
    {
        if(base.Init() == false)
        return false;

        BindButtons(typeof(Buttons));

        GetButton((int)Buttons.Btn_GoogleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Google));
        GetButton((int)Buttons.Btn_AppleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Apple));
        GetButton((int)Buttons.Btn_GuestLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Guest));
        return true;
    }

    private void OnClickLoginButton(ELoginType loginType)
    {
        _backEndAuthService.AuthLogin(loginType);
    }
}
