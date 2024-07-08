using TA.Services;
using TA.UserProfile;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class WalletText : MonoBehaviour{
    UserProfileService _userProfileServie;

    void Start(){
        _userProfileServie = ServiceLocator.Instance.GetService<UserProfileService>();
        
        if(_userProfileServie.LoggedIn){
             GetComponent<TextMeshProUGUI>().text = "username: " + _userProfileServie.UserData.username;
        }
    }
}
