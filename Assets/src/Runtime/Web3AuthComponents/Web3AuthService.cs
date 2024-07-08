using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TA.Services;
using UnityEngine;

namespace TA.Authentication{
public class Web3AuthService : Service<Web3AuthService> {

    [SerializeField] Web3Auth web3Auth;
    Web3AuthConfigProviderService _configProviderService;

    public event Action<Web3AuthResponse> OnLogin;
    public event Action OnLogout;
    public event Action OnWaitingLogin;
    public event Action OnLoginCancelled;

    private CancellationTokenSource _loginCTS = new();

    private Web3AuthResponse _response;
    public Web3AuthResponse LoginResponse {
        get{
            if(_response == null) throw new Exception("Cannot get response, user not logged in");
            return _response;
        }
    }

    bool _loggedIn = false;
    public bool LoggedIn => _loggedIn;

    public async void Login(LoginParams loginParams){

        //following two lines logout the current user
        //if another login is invoked while a user is already logged in
        //and ensures that the login does not take place until the user is logged out
        if(LoggedIn) web3Auth.logout();
        await UniTask.WaitUntil( () => !LoggedIn);

        //creating new cancellation token source before
        //starting login
        ResetLoginCancellationToken();

        OnWaitingLogin?.Invoke();
        web3Auth.login(loginParams);
    }

    public void CancelLogin(){
        _loginCTS.Cancel();
        OnLoginCancelled?.Invoke();
    }

    void ResetLoginCancellationToken(){
        _loginCTS.Dispose();
        _loginCTS = new();
    }

    public void LogOut(){
        web3Auth.logout();
    }

    void Start(){
        _configProviderService = ServiceLocator.Instance.GetService<Web3AuthConfigProviderService>();
        Debug.Log($"Client Id fethced as: {_configProviderService.Config.clientID}");

        web3Auth.setOptions(_configProviderService.SelectedOptions);

        web3Auth.onLogin += OnResponseReceived;

        web3Auth.onLogout += OnWeb3AuthLogout;
    }

    void OnResponseReceived(Web3AuthResponse response){
        OnLoginCancellable(response, _loginCTS.Token);
    }

    void OnLoginCancellable(Web3AuthResponse response, CancellationToken token){
        if(token.IsCancellationRequested) return;
        SaveLogin(response);
        OnLogin?.Invoke(response);
    }

    void SaveLogin(Web3AuthResponse response){
        Debug.Log($"logged in as {response.userInfo.name}");
        _response = response;
        _loggedIn = true;
    }

    void OnWeb3AuthLogout(){
        Debug.Log("logged out of web3 auth service");
        _loggedIn = false;
        OnLogout?.Invoke();
    }

    protected override void OnInitialize(){
        Debug.Log("Web3Auth service initialize");
    }
}
}
