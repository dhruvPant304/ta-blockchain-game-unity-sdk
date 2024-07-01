using TA.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TA.Authentication{
[RequireComponent(typeof(Button))]
public class Web3AuthLogoutButton : MonoBehaviour {
    Web3AuthService _web3AuthService;

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        
        _web3AuthService.OnLogin += (r) => Show();
        _web3AuthService.OnLogout += OnLogout; 

        GetComponent<Button>().onClick.AddListener(LogOut);
    }

    void LogOut(){
        _web3AuthService.LogOut();
    }

    void OnLogout(){
        SceneManager.LoadScene(0);
        ServiceLocator.Instance.CloseServices(); 
    }

    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
}
}
