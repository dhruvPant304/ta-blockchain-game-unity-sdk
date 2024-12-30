using UnityEngine;
using TMPro;
using TA.UserProfile;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserProfile.Balance;
using Cysharp.Threading.Tasks;

namespace TA.Components{
[RequireComponent(typeof(TextMeshProUGUI))]
public class BalanceText : MonoBehaviour{
    TextMeshProUGUI textMesh;
    UserBalanceService _userBalanceService;
    [SerializeField] string currency = "credits";

    void Start(){
        textMesh = GetComponent<TextMeshProUGUI>();

        ServiceLocator.Instance.OnServiceRegistered<UserBalanceService>(
            async () => {

                var profile = ServiceLocator.Instance.GetService<UserProfileService>();
                await UniTask.WaitUntil(() => profile.LoggedIn);
                _userBalanceService = ServiceLocator.Instance.GetService<UserBalanceService>();
                _userBalanceService.OnBalanceSync += OnBalanceUpdate;
                _userBalanceService.OnBalanceSyncFailed += OnBalanceFailed;

               OnBalanceUpdate(_userBalanceService.LastSyncedBalance());
            }
        );
    }

    void OnBalanceUpdate(UserBalanceData data){
        if(data == null) {
            OnBalanceFailed();
            return;
        }
        var amount = data.GetFild<int>(currency);
        if(amount == null) {
            OnBalanceFailed();
            return;
        }
        textMesh.text = data.credits.ToString();
    }

    void OnBalanceFailed(){
        textMesh.text = "Error!";
    }
}}
