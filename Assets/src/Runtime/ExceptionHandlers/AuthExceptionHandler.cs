using System.Collections.Generic;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Authentication;
using TA.Components;
using TA.Services;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TA.ExceptionHandling{
public class AuthExceptionHandler : MonoBehaviour{

    UserProfileService _profileService;
    Web3AuthService _web3AuthService;
    BlockchainGameCanvas _blockChainGameCanvas;
    APIConfig _apiConfig;

    async void Start(){
        _profileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _blockChainGameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
        _apiConfig = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;
        
        _profileService.OnAuthFailed += OnAuthFailed;
        _profileService.OnAuthSuccess += OnAuthSuccess;

        if(UserProfileService.HasSavedLoginSession()) {
            if(!await _profileService.TryStartSavedLoginSession()){
                //Jumping back to landing page if game fails to Start saved session
                SceneManager.LoadScene(0);
            }
        }
        Debug.Log("Auth exception handler ready");
    }

    void OnAuthFailed(FailedResponse failedResponse){
        var errorPopup = new MessagePopup(){
            header = "Something went wrong!!",
            message = failedResponse.message,
            banner = BannerType.Danger,
            hasBackground = true,
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit(){
                    name = "Try Again",
                    exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                    exitAction = () => {
                        _web3AuthService.LogOut();
                    }
                }
            }
        };

        _blockChainGameCanvas.ShowMessagePopup(errorPopup);
    }

    void OnAuthSuccess(LoginSessionData userData){
        if(!userData.isFirstTimeUser 
                || _profileService.IsAutoLoginSession ) {
            if(!userData.isFirstTimeUser) Debug.Log("not a first time user");
            SceneManager.LoadScene(3);
            _profileService.OnAuthSuccess -= OnAuthSuccess;
            return;
        }
        var popup = new MessagePopup(){
            header = _apiConfig.firstLoginMessageHeader,
            message = _apiConfig.firstLoginMessage,
            banner = BannerType.Reward,
            hasBackground = true,
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit{
                    name = "Okay",
                    exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                    exitAction = () => {
                        SceneManager.LoadScene(3);
                        _profileService.OnAuthSuccess -= OnAuthSuccess;
                    }
                }
            }
        };

        _blockChainGameCanvas.ShowMessagePopup(popup);
    }
}
}
