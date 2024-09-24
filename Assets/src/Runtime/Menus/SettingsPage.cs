using UnityEngine;
using TMPro;
using TA.UserProfile;
using TA.Services;
using System.Linq;
using TA.Authentication;
using TA.Components;
using System.Collections.Generic;

namespace TA.Menus{
public class SettingsPage : MonoBehaviour {
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI email;
    [SerializeField] SliderToggle soundToggle;
    [SerializeField] SliderToggle vibrationsToggle;
    [SerializeField] Web3AuthLogoutButton deleteLogout;

    UserProfileService _userProfileService;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        Hide();
    }

    void OnShow(){
        var userData = _userProfileService.SessionUserData;
        if(playerName) playerName.text = userData.username;
        if (email) email.text = userData.email;

        if(soundToggle){
            soundToggle.SetValueWithoutNotify(userData.appSettings.Last().isMusic);
            soundToggle.OnValueChanged = (val) => UpdateSettings();
        }

        if(vibrationsToggle){
            vibrationsToggle.SetValueWithoutNotify(userData.appSettings.Last().isVibrate);
            vibrationsToggle.OnValueChanged = (val) => UpdateSettings();
        }

        if(deleteLogout){
            deleteLogout.onLogout = DeleteUser;
            deleteLogout.confirmationPopUp = DeleteConfirmationPopUp();
        }
    }

    public void Show(){ 
        gameObject.SetActive(true);
        OnShow();
    }

    void UpdateSettings(){
        _userProfileService.UpdateUserSettings(soundToggle.Value, false, vibrationsToggle.Value);
    }

    public void Hide(){
        gameObject.SetActive(false);
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
}}
