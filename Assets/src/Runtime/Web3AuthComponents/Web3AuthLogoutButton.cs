using TA.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TA.UserProfile;
using System;

namespace TA.Authentication{
[RequireComponent(typeof(Button))]
public class Web3AuthLogoutButton : MonoBehaviour {
    Web3AuthService _web3AuthService;
    UserProfileService _userProfileService;
    public Action onLogout;

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();

        _web3AuthService.OnLogin += (r) => Show();
        _web3AuthService.OnLogout += OnLogout; 

        GetComponent<Button>().onClick.AddListener(LogOut);
    }

    void LogOut(){
        _web3AuthService.LogOut();
    }

    void OnLogout(){
        onLogout?.Invoke();
        SceneManager.LoadScene(0);
        _userProfileService.ClearLoginSession();
        ServiceLocator.Instance.CloseServices();
    }

    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
}
}
