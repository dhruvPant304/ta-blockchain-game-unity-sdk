using TA.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TA.UserProfile;
using System;
using TA.Components;
using Cysharp.Threading.Tasks;

namespace TA.Authentication{
[RequireComponent(typeof(Button))]
public class Web3AuthLogoutButton : MonoBehaviour {
    Web3AuthService _web3AuthService;
    UserProfileService _userProfileService;
    BlockchainGameCanvas _gameCanvas;

    public Action onLogout;
    public MessagePopup confirmationPopUp;
    public MessagePopup successPopUp; 

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _gameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();

        _web3AuthService.OnLogin += (r) => Show();
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick(){
        if(confirmationPopUp !=null){
            foreach (var exit in confirmationPopUp.exits){
                //Regular buttons would act as confirmation to Execute On Logout logic
                //This was done to give cancellation button in Delete account pop-up a
                //confirmation style highlight
                if(exit.exitStyle == MessagePopupExit.ExitStyle.Regular){ 
                    exit.exitAction = LogOut;
                }
        }

            _gameCanvas.ShowMessagePopup(confirmationPopUp);
            return;
        }
        LogOut();
    }

    void LogOut(){
        Debug.Log("Logging out...");
        if(_userProfileService.IsAutoLoginSession){
            OnLogout();
            return;
        }

        _web3AuthService.OnLogout += OnLogout; 
        _web3AuthService.LogOut();
    }

    async void ShowLogOutSuccessPopUp(){
         if(successPopUp !=null){
            await UniTask.WaitForSeconds(0.5f);
            _gameCanvas.ShowMessagePopup(successPopUp);
            return;
        }
    }

    void OnLogout(){
        onLogout?.Invoke();
        _userProfileService.ClearLoginSession();
        SceneManager.LoadScene(0);
        _web3AuthService.OnLogout -= OnLogout;
        ShowLogOutSuccessPopUp();
    }

    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
}
}
