using System;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Authentication;
using TA.Helpers.Crypto;
using TA.Services;
using UnityEngine;

namespace TA.UserProfile{
public class UserProfileService : Service<UserProfileService>{

    protected override void OnInitialize(){
    }

    Web3AuthService _web3AuthService;
    APIService _apiService;
    string platform = "android";
    string appType = "etuktuk";

    LoginUserData _userData;
    public LoginUserData UserData => _userData;

    UserBalanceData _userBalanceData;
    public UserBalanceData UserBalanceData => _userBalanceData;

    //Events
    public Action<LoginUserData> OnAuthSuccess;
    public Action<FailedResponse>  OnAuthFailed;
    public Action OnAuthComplete;
    public Action<UserBalanceData> OnBalanceUpdate;
    public Action OnBalanceUpdateFailed;

    private bool _loggedIn;
    public bool LoggedIn => _loggedIn;

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _apiService = ServiceLocator.Instance.GetService<APIService>();

        _web3AuthService.OnLogin += OnLogin;
    }

    void OnDisable(){
        _web3AuthService.OnLogin -= OnLogin;
    }

    async void OnLogin(Web3AuthResponse response){
        var privateKey = response.privKey;

        var walletAddress = CryptoHelper.GetWalletAddress(privateKey);
        var signature = CryptoHelper.GetMessageSignature("etuktuk", privateKey);

        var loginParams = new TA.APIClient.RequestData.LoginParams(){
            verifierId = response.userInfo.verifierId,
            loginType = response.userInfo.typeOfLogin,
            walletAddress = walletAddress,
            signature = signature,
            email = response.userInfo.email,
            platform = this.platform,
            appType = this.appType
        };

        var requestResponse = await _apiService.SendLoginRequest(loginParams);

        if(requestResponse.IsSuccess) 
            HandleAuthSuccess(requestResponse.SuccessResponse);
        else 
            HandleAuthFailiure(requestResponse.FailureResponse);

        _loggedIn = true;
        OnAuthComplete?.Invoke();
    }

    void HandleAuthSuccess(LoginResponse response){
         var loginData = response.data;
        _userData = loginData;

        OnAuthSuccess?.Invoke(_userData);
        UpdateUserBalance().Forget();
    }

    void HandleAuthFailiure(FailedResponse failedResponse){
        Debug.LogError($"Failed to authentiacate user: {failedResponse.message}");
        OnAuthFailed?.Invoke(failedResponse);
    }

    public async UniTask<StaticRequestResponse<UserBalanceResponse>> UpdateUserBalance(){
        var response = await _apiService.SendFetchUserBalanceRequest(UserData.token);
        if(response.IsSuccess){
            OnBalanceUpdate?.Invoke(response.Response.data);
            _userBalanceData = response.Response.data;
        } else{
            OnBalanceUpdateFailed?.Invoke();
        }
        return response;
    }
}
}
