using TA.Authentication;
using TA.Services;
using TA.UserProfile;
using UnityEngine;

namespace TA.Components{
public class LoginPanel : MonoBehaviour{
    [SerializeField] GameObject loginPanelBackGround;
    Web3AuthService _webAuthService;
    UserProfileService _userProfileService;
    CanvasGroupFader _canvasGroupFader;

    void Start(){
        _webAuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _canvasGroupFader = ServiceLocator.Instance.GetService<CanvasGroupFader>();
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _userProfileService.OnAuthComplete += Hide;

        if(_userProfileService.HasSavedLoginSession()){
            Hide();
        }
        else {
            _webAuthService.OnWaitingLogin += Hide;
            _webAuthService.OnLogout += Show;
            _webAuthService.OnLoginCancelled += Show;
            Show();
        }
    }

    void OnDestroy(){
        _webAuthService.OnWaitingLogin -= Hide;
        _userProfileService.OnAuthComplete -= Hide;
        _webAuthService.OnLogout -= Show;
        _webAuthService.OnLoginCancelled -= Show;
    }

    void Show(){
        gameObject.SetActive(true);
        if(loginPanelBackGround) loginPanelBackGround.SetActive(true);
    }

    void OnLoginHide(Web3AuthResponse r){
        Hide();
    }

    void Hide(){
        gameObject.SetActive(false);
        if(loginPanelBackGround) loginPanelBackGround.SetActive(false);
    }
}
}
