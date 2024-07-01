using System;
using System.Globalization;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserProfile;
using TMPro;
using UnityEngine;

namespace TA.Menus{
public class ProfileDetailsPage : MonoBehaviour{
    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TextMeshProUGUI walletAddress;
    [SerializeField] TextMeshProUGUI email;
    [SerializeField] TextMeshProUGUI createdAt;

    UserProfileService _userProfileService;
    LoginUserData _userData;

    void Start(){
        Hide();
    }

    void SaveUserData(LoginUserData data){
        _userData = data;
    }

    public void Show(){
        gameObject.SetActive(true);
        _userData = ServiceLocator.Instance.GetService<UserProfileService>().UserData;
        userName.text = _userData.username;
        walletAddress.text = GetTruncatedString(6,_userData.walletAddress);
        email.text = _userData.email;
        createdAt.text = "Member scince," + ConvertToCustomFormat(_userData.createdAt);
    }

    public void Hide(){
        gameObject.SetActive(false);
    }

    string GetTruncatedString(int characters, string str){
        if(str.Length <= 2*characters) return str;
        return str.Substring(0, characters) + "..." + str.Substring(str.Length - characters, characters);
    }

    string ConvertToCustomFormat(string inputDate){
        DateTime dateTime = DateTime.Parse(inputDate, null, DateTimeStyles.RoundtripKind);
        string formattedDate = dateTime.ToString("dd MMMM yyyy", CultureInfo.InvariantCulture);
        return formattedDate;
    }
}
}
