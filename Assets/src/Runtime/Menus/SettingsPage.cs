using UnityEngine;
using TMPro;
using TA.UserProfile;
using TA.Services;
using System.Linq;
using TA.Authentication;

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
        playerName.text = userData.username;
        email.text = userData.email;

        soundToggle.SetValueWithoutNotify(userData.appSettings.Last().isMusic);
        vibrationsToggle.SetValueWithoutNotify(userData.appSettings.Last().isVibrate);

        soundToggle.OnValueChanged = (val) => UpdateSettings();
        vibrationsToggle.OnValueChanged = (val) => UpdateSettings();
        deleteLogout.onLogout = DeleteUser;
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

    void DeleteUser(){
        _userProfileService.DeleteUser();
    }
}}
