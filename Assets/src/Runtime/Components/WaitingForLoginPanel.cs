using TA.Authentication;
using TA.Services;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Components{
public class WaitingForLoginPanel : MonoBehaviour {
    [SerializeField] Button cancelButton;
    
    Web3AuthService _web3AuthService;
    UserProfileService _userProfile;
    CanvasGroupFader _canvasGroupFader;

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _userProfile = ServiceLocator.Instance.GetService<UserProfileService>();
        _canvasGroupFader = ServiceLocator.Instance.GetService<CanvasGroupFader>();

        _web3AuthService.OnWaitingLogin += Show;
        _web3AuthService.OnLoginCancelled += Hide;
        _userProfile.OnAuthComplete += Hide;

        cancelButton.onClick.AddListener(CancelLogin);
        Hide();
    }

    void OnDestroy(){
        _web3AuthService.OnWaitingLogin -= Show;
        _web3AuthService.OnLoginCancelled -= Hide;
        _web3AuthService.OnLogin -= OnLoginHide; 
    }

    void OnLoginHide(Web3AuthResponse r){
        Hide();
    }

    void CancelLogin(){
        _web3AuthService.CancelLogin();
    }

    void Hide(){
        //await _canvasGroupFader.FadeOut();
        gameObject.SetActive(false);
    }

    void Show(){
        //await _canvasGroupFader.FadeIn();
        gameObject.SetActive(true);
    }
}
}

