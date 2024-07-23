using System;
using System.Globalization;
using TA.APIClient.ResponseData;
using TA.Components;
using TA.Services;
using TA.UserProfile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class ProfileDetailsPage : MonoBehaviour{
    [SerializeField] TextMeshProUGUI userName;
    [SerializeField] TextMeshProUGUI walletAddress;
    [SerializeField] TextMeshProUGUI email;
    [SerializeField] TextMeshProUGUI createdAt;
    [SerializeField] Button editProfileButton;
    [SerializeField] InputModal editProfileInputModal;

    UserProfileService _userProfileService;

    void Start(){
        Hide();
    }

    public void Show(){
        gameObject.SetActive(true);
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        var userData = _userProfileService.SessionUserData;

        UpdateData(userData);
        _userProfileService.OnUserDataUpdate += UpdateData;
        editProfileButton.onClick.AddListener(OnEditProfile);
    }

    void UpdateData(UserData data){
        userName.text = data.username;
        walletAddress.text = GetTruncatedString(6,data.walletAddress);
        email.text = data.email;
        createdAt.text = "Member since," + ConvertToCustomFormat(data.createdAt);
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

    async void OnEditProfile(){
        var result = await editProfileInputModal.WaitInput("Edit Profile");
        if(result.option == Option.Some){
            _userProfileService.UpdateUserName(result.result);
        }
    }
}
}
