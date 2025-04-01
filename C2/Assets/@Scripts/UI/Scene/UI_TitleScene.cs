using C2Project.BackEnd;
using UnityEngine;
using Zenject;
using C2Project.Model;
using C2Project.Signals;
using static Define;
using TMPro;
using UnityEngine.UI;
using UniRx;


public class UI_TitleScene : UI_Base
{
    public enum CanvasGroups
    {
        LoginButtonGroup,
        LoadDataGroup
    }

    public enum Sliders 
    {
        LoadSlider
    }

    public enum Texts 
    {
        LoadProgressText
    }


    public enum Buttons
    {
        Btn_GoogleLogin,
        Btn_AppleLogin,
        Btn_GuestLogin
    }

    [Inject] private BackEndAuthService _backEndAuthService;
    [Inject] private BackEndTableSerivce _backEndTableSerivce;
    [Inject] private PlayerModel _playerModel;

    [Inject] private PopupService _popupService;
    [Inject] private SignalBus _signalBus;

    private CanvasGroup _loginBtnGroup;
    private CanvasGroup _loadDataGroup;

    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        _popupService.SetCurrentSceneUI(this);
        
        BindSliders(typeof(Sliders));
        BindTMPTexts(typeof(Texts));
        BindButtons(typeof(Buttons));
        Bind<CanvasGroup>(typeof(CanvasGroups));

        _loginBtnGroup = Get<CanvasGroup>((int)CanvasGroups.LoginButtonGroup);
        _loadDataGroup = Get<CanvasGroup>((int)CanvasGroups.LoadDataGroup);

        _loginBtnGroup.SetCanvasGroupState(false,false);
        _loadDataGroup.SetCanvasGroupState(false,false);

        GetButton((int)Buttons.Btn_GoogleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Google));
        GetButton((int)Buttons.Btn_AppleLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Apple));
        GetButton((int)Buttons.Btn_GuestLogin).onClick.AddListener(() => OnClickLoginButton(ELoginType.Guest));

        _signalBus.Subscribe<LoginSuccessSignal>(() => _loadDataGroup.SetCanvasGroupState(true, false));
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

    public void BindProgress(float progressValue)
    {
        GetSlider((int)Sliders.LoadSlider).value = progressValue;
        GetTMPText((int)Texts.LoadProgressText).text = $"{progressValue * 100f:0}%";
    }

}
