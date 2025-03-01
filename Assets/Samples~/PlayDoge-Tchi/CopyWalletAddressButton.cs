using TA.Services;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.UI;

public class CopyWalletAddressButton : MonoBehaviour{
    [SerializeField] Button copyButton;

    UserProfileService _userProfileService;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        copyButton.onClick.AddListener(CopyAddress);
    }

    void CopyAddress(){
        GUIUtility.systemCopyBuffer = _userProfileService.LoginUserData.walletAddress;
    }
}
