using System.Collections.Generic;
using TA.APIClient.ResponseData;
using TA.Authentication;
using TA.Components;
using TA.Services;
using TA.UserProfile;
using UnityEngine;

namespace TA.ExceptionHandling{
public class AuthExceptionHandler : MonoBehaviour{
    UserProfileService _profileService;
    Web3AuthService _web3AuthService;
    BlockchainGameCanvas _blockChainGameCanvas;

    void Start(){
        _profileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _blockChainGameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();

        _profileService.OnAuthFailed += OnAuthFailed;
        _profileService.OnAuthSuccess += OnAuthSuccess;
    }

    void OnAuthFailed(FailedResponse failedResponse){
        var errorPopup = new MessagePopup(){
            header = "Something went wrong!!",
            message = "Failed to authenticate user" + failedResponse.message,
            banner = BannerType.Danger,
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

    void OnAuthSuccess(LoginUserData userData){
        if(!userData.isFirstTimeUser) return;
        var popup = new MessagePopup(){
            header = "Login Successful!",
            message = "Welcome to TukTuk Crazy Taxi, We have 20 free credits for you",
            banner = BannerType.Reward,
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit{
                    name = "Okay",
                    exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                    exitAction = () => {
                        //Does nothing
                    }
                }
            }
        };

        _blockChainGameCanvas.ShowMessagePopup(popup);
    }
}
}
