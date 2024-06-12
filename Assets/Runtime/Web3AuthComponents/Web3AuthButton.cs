using UnityEngine;
using UnityEngine.UI;

namespace TA.Authentication{
[RequireComponent(typeof(Button))]
public class Web3AuthButton : MonoBehaviour{
    [SerializeField] Provider loginProvider;
    Web3AuthService web3AuthService;

    public void Start(){
        web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        web3AuthService.OnLogin += OnLoggedIn;
        GetComponent<Button>().onClick.AddListener(Login);
    }

    void Login(){
        var loginParams = new LoginParams(){
            loginProvider = loginProvider,
        };
        web3AuthService.Web3Auth.login(loginParams);
    }

    void OnLoggedIn(Web3AuthResponse response){
        Debug.Log($"Logged in through login provider {loginProvider}");
    }
}
}
