using C2Project.BackEnd;
using UnityEngine;
using Zenject;
using RSG;
using static Define;
using C2Project.Signals;


public class UI_TitleScene : UI_Base
{
    public enum CanvasGroups
    {
        LoginButtonGroup
    }

    public enum Buttons
    {
        Btn_GoogleLogin,
        Btn_AppleLogin,
        Btn_GuestLogin
    }

    [Inject] private BackEndAuthService _backEndAuthService;
    [Inject] private PopupService _popupService;
    [Inject] private SignalBus _signalBus;

    private CanvasGroup _loginBtnGroup;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _popupService.SetCurrentSceneUI(this);
        
         
        Bind<CanvasGroup>(typeof(CanvasGroups));
        BindButtons(typeof(Buttons));

        _loginBtnGroup = Get<CanvasGroup>((int)CanvasGroups.LoginButtonGroup);
        _loginBtnGroup.SetCanvasGroupState(false,false);

        GetButton((int)Buttons.Btn_GoogleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Google));
        GetButton((int)Buttons.Btn_AppleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Apple));
        GetButton((int)Buttons.Btn_GuestLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Guest));
        return true;
    }


    public void ActiveLoginButtonGroup(bool success)
    {
        _loginBtnGroup.SetCanvasGroupState(success, false);

        if(success)
        CheckEditorAutoButton();
    }

    private void CheckEditorAutoButton()
    {
        GetButton((int)Buttons.Btn_GoogleLogin).gameObject.SetActive(false);
        GetButton((int)Buttons.Btn_AppleLogin).gameObject.SetActive(false);

#if UNITY_ANDROID
        GetButton((int)Buttons.Btn_GoogleLogin).gameObject.SetActive(true);
#elif UNITY_IOS
        GetButton((int)Buttons.Btn_AppleLogin).gameObject.SetActive(true);
#endif
    }

    private void OnClickLoginButton(ELoginType loginType)
    {
        _backEndAuthService.AuthLogin(loginType)
        .Then(result => 
        {
            if(result)
            {
                _signalBus.Fire(new LoginSuccessSignal());
                _loginBtnGroup.SetCanvasGroupState(false, false);
            }
        });
    }
}
