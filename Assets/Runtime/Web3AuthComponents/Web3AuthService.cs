using System;
using UnityEngine;

namespace TA.Authentication{
public class Web3AuthService : Service<Web3AuthService> {
    [SerializeField] Web3Auth web3Auth;
    Web3AuthConfigProviderService _configProviderService;

    public event Action<Web3AuthResponse> OnLogin;
    public event Action OnLogout;
    public Web3Auth Web3Auth => web3Auth;

    void Start(){
        _configProviderService = ServiceLocator.Instance.GetService<Web3AuthConfigProviderService>();
        Debug.Log($"Client Id fethced as: {_configProviderService.Config.clientID}");

        web3Auth.setOptions(_configProviderService.SelectedOptions);
        web3Auth.onLogin += OnLogin;
        web3Auth.onLogout += OnLogout;
    } 

    void OnDisable(){
        web3Auth.onLogin -= OnLogin;
        web3Auth.onLogout -= OnLogout;
    }

    protected override void OnInitialize(){
        Debug.Log("Web3Auth service initialize");
    }
}
}
