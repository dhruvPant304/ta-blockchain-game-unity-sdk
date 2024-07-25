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
        GetComponent<Button>().onClick.AddListener(LogOut);
    }

    void LogOut(){
        Debug.Log("Logging out...");
        if(_userProfileService.IsAutoLoginSession){
            OnLogout();
            return;
        }

        _web3AuthService.OnLogout += OnLogout; 
        _web3AuthService.LogOut();
        _web3AuthService.OnLogout -= OnLogout;
    }

    void OnLogout(){
        onLogout?.Invoke();
        _userProfileService.ClearLoginSession();
        SceneManager.LoadScene(0);
    }

    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
}
}
