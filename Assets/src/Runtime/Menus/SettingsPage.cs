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
    BlockchainGameCanvas _gameCanvas;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _gameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();

        Hide();
    }

    void OnShow(){
        var userData = _userProfileService.SessionUserData;
        playerName.text = userData.username;
        email.text = userData.email;

        soundToggle.SetValueWithoutNotify(userData.appSettings.Last().isMusic);
        vibrationsToggle.SetValueWithoutNotify(userData.appSettings.Last().isVibrate);

        soundToggle.OnValueChanged = (val) => UpdateSettings();
        vibrationsToggle.OnValueChanged = (val) => UpdateSettings();
        deleteLogout.onLogout = ShowDeleteUser;
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

    void ShowDeleteUser(){
        var popup = new MessagePopup{
            hasBackground = true,
            banner = BannerType.None,
            header = "Leaving Us?",
            message = "Are you sure you want to delete your account? your details will be removed.",
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit(){
                    name = "Cancel",
                    exitStyle = MessagePopupExit.ExitStyle.Regular,
                    exitAction = null
                },
                new MessagePopupExit(){
                    name = "Delete",
                    exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                    exitAction = DeleteUser
                }
            }
        };

        _gameCanvas.ShowMessagePopup(popup);
    }

    void DeleteUser(){
        _userProfileService.DeleteUser();
    }
}}
