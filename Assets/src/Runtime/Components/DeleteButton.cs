using System.Collections.Generic;
using TA.Authentication;
using TA.Services;
using TA.UserProfile;
using UnityEngine;

namespace TA.Components{
    [RequireComponent(typeof(Web3AuthLogoutButton))]
    public class DeleteButton : MonoBehaviour {
        UserProfileService _userProfileService;

        void Start(){
            _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
            var logoutComponent = GetComponent<Web3AuthLogoutButton>();

            if(logoutComponent){
                logoutComponent.onLogout = DeleteUser;
                logoutComponent.confirmationPopUp = DeleteConfirmationPopUp();
            }
        }

        MessagePopup DeleteConfirmationPopUp(){
            var popup = new MessagePopup{
                hasBackground = true,
                banner = BannerType.None,
                header = "Leaving Us?",
                message = "Are you sure you want to delete your account? your details will be removed.",
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

        void DeleteUser(){
            _userProfileService.DeleteUser();
        }

    }
}

