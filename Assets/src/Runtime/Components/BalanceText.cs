using UnityEngine;
using TMPro;
using TA.UserProfile;
using TA.APIClient.ResponseData;
using TA.Services;

namespace TA.Components{
[RequireComponent(typeof(TextMeshProUGUI))]
public class BalanceText : MonoBehaviour{
    TextMeshProUGUI textMesh;
    UserProfileService _userProfileService;
    [SerializeField] string currency = "credits";

    void Start(){
        textMesh = GetComponent<TextMeshProUGUI>();

        ServiceLocator.Instance.OnServiceRegistered<UserProfileService>(
            () => {
                _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
                _userProfileService.OnBalanceUpdate += OnBalanceUpdate;
                _userProfileService.OnBalanceUpdateFailed += OnBalanceFailed;

               OnBalanceUpdate(_userProfileService.UserBalanceData);
            }
        );
    }

    void OnBalanceUpdate(UserBalanceData data){
        if(data == null) OnBalanceFailed();
        var amount = data.GetFild<int>(currency);
        if(amount == null) {
            textMesh.text = "ERROR";
            return;
        }
        textMesh.text = data.credits.ToString();
    }

    void OnBalanceFailed(){
        textMesh.text = "Error!";
    }
}}
