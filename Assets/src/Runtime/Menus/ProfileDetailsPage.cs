using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using TA.APIClient.ResponseData;
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
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _userProfileService.OnUserDataUpdate += UpdateData;
        editProfileButton.onClick.AddListener(OnEditProfile);

        var userData = _userProfileService.SessionUserData;
        UpdateData(userData);
    }

    public void Show(){
        gameObject.SetActive(true);
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
        var result = await editProfileInputModal.WaitInput("Edit Profile", "name",ValidateUserName);
        if(result.option == Option.Some){
            _userProfileService.UpdateUserName(result.result);
        }
    }

    async UniTask<ValidationResult> ValidateUserName(string username){
        return IsValid(username);
    }

    public ValidationResult IsValid(string playerName) {
        if (playerName.Length < 3) {
            return new ValidationResult { IsValid = false, 
                ErrorMessage = "Player name must be a minimum of 3 characters." };
        }
        if (playerName.Length > 25) {
            return new ValidationResult { IsValid = false, 
                ErrorMessage = "Player name must be a maximum of 25 characters." };
        }

        if (!Regex.IsMatch(playerName, "^[a-zA-Z0-9]+$")) {
            return new ValidationResult { IsValid = false, 
                ErrorMessage = "Special characters are not allowed" };
        }

        if (new ProfanityFilter.ProfanityFilter().ContainsProfanity(playerName)) {
            return new ValidationResult { IsValid = false, 
                ErrorMessage = "Player name contains inappropriate language." };
        }

        return new ValidationResult { IsValid = true, 
            ErrorMessage = "Player name is valid." };
    }
}
}
