using System.Collections.Generic;
using TA.APIClient;
using TA.Authentication;
using TA.Services;
using TA.UserProfile;
using UnityEngine;

namespace TA.Components{
    [RequireComponent(typeof(Web3AuthLogoutButton))]
    public class DeleteButton : MonoBehaviour {
        UserProfileService _userProfileService;
        APIConfig _apiConfig;

        void Start(){
            _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
            _apiConfig = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;

            var logoutComponent = GetComponent<Web3AuthLogoutButton>();

            if(logoutComponent){
                logoutComponent.onLogout = DeleteUser;
                if(_apiConfig.showDeleteConfirmationPopUp) logoutComponent.confirmationPopUp = DeleteConfirmationPopUp();
                if(_apiConfig.showDeleteSuccessPopUp) logoutComponent.successPopUp = DeleteSuccessPopUp();
            }
        }

        MessagePopup DeleteConfirmationPopUp(){
            var popup = new MessagePopup{
                hasBackground = true,
                banner = BannerType.None,
                header = _apiConfig.deleteConfirmationMessageHeader,
                message = _apiConfig.deleteConfirmationMessage, 
                exits = new List<MessagePopupExit>(){
                    new MessagePopupExit(){
                        name = "Cancel",
                        exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                        exitAction = null
                    },
                    new MessagePopupExit(){
                        name = "Delete",
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                    }
                }
            };

            return popup;
        }

        MessagePopup DeleteSuccessPopUp(){
            var popup = new MessagePopup{
                hasBackground = true,
                banner = BannerType.Good,
                header = _apiConfig.deleteSuccessMessage,
                message = _apiConfig.deleteSuccessMessageHeader,
                exits = new List<MessagePopupExit>(){
                    new MessagePopupExit(){
                        name = "Okay",
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                    }
                }
            };

            return popup;
        }

        void DeleteUser(){
            _userProfileService.DeleteUser();
        }

    }
}

