using UnityEngine;
using TMPro;
using TA.UserProfile;
using TA.Services;

namespace TA.Menus{
public class SettingsPage : MonoBehaviour {
    [SerializeField] TextMeshProUGUI playerName; 
    [SerializeField] TextMeshProUGUI email; 
    [SerializeField] SliderToggle soundToggle;
    [SerializeField] SliderToggle vibrationsToggle;

    UserProfileService _userProfileService;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        Hide();
    }

    void OnShow(){
        var userData = _userProfileService.UserData;
        playerName.text = userData.username;
        email.text = userData.email;
        soundToggle.SetValueWithoutNotify(userData.appSettings[0].isSound);
        vibrationsToggle.SetValueWithoutNotify(userData.appSettings[0].isVibrate);
    }

    public void Show(){ 
        gameObject.SetActive(true);
        OnShow();
    }

    public void Hide(){
        gameObject.SetActive(false);
    }
}}