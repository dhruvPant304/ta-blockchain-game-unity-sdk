using System;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Authentication;
using TA.Helpers.Crypto;
using TA.Services;
using UnityEngine;
using Newtonsoft.Json;
using TA.Components;
using System.Collections.Generic;
using TA.APIClient.RequestData;
using TA.UserProfile.Balance;

namespace TA.UserProfile{
public class UserProfileService : Service<UserProfileService>{

    protected override void OnInitialize(){}

    Web3AuthService _web3AuthService;
    APIService _apiService;
    BlockchainGameCanvas _gameCanvas;
    APIConfigProviderService _apiConfigService;
    UserBalanceService _balanceService;

    string platform = "android";
    string appType = "etuktuk";

    LoginSessionData _userloginData;
    public LoginSessionData LoginUserData => _userloginData;

    public string LoginToken => _userloginData.token;

    UserData _sessionUserData;
    public UserData SessionUserData => _sessionUserData;
 
    //Events
    public Action OnAuthComplete;
    public Action<LoginSessionData> OnAuthSuccess;
    public Action<FailedResponse>  OnAuthFailed;
    public Action<UserData> OnUserDataUpdate;
    
    public bool LoggedIn {get; private set;}
    public bool IsAutoLoginSession {get; private set;}

    void Start(){
        _web3AuthService = ServiceLocator.Instance.GetService<Web3AuthService>();
        _apiService = ServiceLocator.Instance.GetService<APIService>();
        _gameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
        _apiConfigService = ServiceLocator.Instance.GetService<APIConfigProviderService>();
        _balanceService = ServiceLocator.Instance.GetService<UserBalanceService>();

        _web3AuthService.OnLogin += OnLogin;
    }

    void OnDisable(){
        _web3AuthService.OnLogin -= OnLogin;
    }

    //=======================
    // WEB3AUTH LOGIN
    //=======================

    async void OnLogin(Web3AuthResponse response){
        var privateKey = response.privKey;

        var walletAddress = CryptoHelper.GetWalletAddress(privateKey);
        var signature = CryptoHelper.GetMessageSignature($"{_apiConfigService.APIConfig.signatureString} Wallet address: {walletAddress}", privateKey);

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

        if(requestResponse.IsSuccess) {
            TryHandleLogin(requestResponse.SuccessResponse.data).Forget();
            SaveLoginSession(requestResponse.SuccessResponse.data);
        }
        else 
            HandleAuthFailiure(requestResponse.FailureResponse);

        OnAuthComplete?.Invoke();
    }

    async UniTask<bool> TryHandleLogin(LoginSessionData response){
         var loginData = response;
        _userloginData = loginData;
        _sessionUserData = loginData; 

        var fetchOnLogin = _apiConfigService.APIConfig.fetchUserBalanceOnLogin;
        var balanceResponse =  fetchOnLogin? 
            await _balanceService.UpdateUserBalance() : 
            APIService.CreateBaseResponse(true, "Not checking for user balance on login");

        if(balanceResponse.IsSuccess){
            OnAuthSuccess?.Invoke(_userloginData);
            OnUserDataUpdate?.Invoke(_userloginData);
            LoggedIn = true;
            return true;
        }

        return false;
    }

    void HandleAuthFailiure(FailedResponse failedResponse){
        Debug.LogError($"Failed to authentiacate user: {failedResponse.message}");
        OnAuthFailed?.Invoke(failedResponse);
    } 

    //=======================
    // SAVED LOGIN SESSIONS
    //=======================

    public const string LOGIN_SESSION_KEY = "LOGIN_SESSION_KEY";

    public async UniTask<bool> TryStartSavedLoginSession(){
        var loginData = JsonConvert.DeserializeObject<LoginSessionData>(PlayerPrefs.GetString(LOGIN_SESSION_KEY));
        if(!await TryHandleLogin(loginData)){
            ClearLoginSession();
            return false;
        }
        return true;
    }

    void SaveLoginSession(LoginSessionData loginData){
        PlayerPrefs.SetString(LOGIN_SESSION_KEY, JsonUtility.ToJson(loginData));
        PlayerPrefs.Save();
    }

    public void ClearLoginSession(){
        PlayerPrefs.DeleteKey(LOGIN_SESSION_KEY);
        PlayerPrefs.Save();
    }

    public static bool HasSavedLoginSession(){
        return PlayerPrefs.HasKey(LOGIN_SESSION_KEY);
    }

    //=======================
    // USER SETTINGS
    //=======================
    
    public Action<bool> OnMusicSettingChanged;
    public Action<bool> OnSoundSettingChanged;
    public Action<bool> OnVibrationSetttingChanged;

    public async void UpdateUserSettings(bool isMusic, bool isSound, bool isVibrate){
        OnMusicSettingChanged?.Invoke(isMusic);
        OnSoundSettingChanged?.Invoke(isSound);
        OnVibrationSetttingChanged?.Invoke(isVibrate);

        var appSettings = new AppSettings {
            id = "0",
            isMusic = isMusic,
            isSound = isSound,
            isVibrate = isVibrate
        };

        var response = await _apiService.SendUpdateUserSettingsRequest(appSettings, _userloginData.token);
        if(response.IsSuccess){
            _sessionUserData = response.SuccessResponse.data;
            OnUserDataUpdate?.Invoke(_sessionUserData);
        }
        else{
            var popup = new MessagePopup{
                header= "Failed to save settings",
                message= response.FailureResponse.message,
                banner = BannerType.Danger,
                exits = new List<MessagePopupExit>{
                    new MessagePopupExit{
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                        name = "okay",
                        exitAction = () => {}
                    }
                }
            };

            _gameCanvas.ShowMessagePopup(popup);
        }
    }

    //=======================
    // UPDATE PROFILE
    //=======================

    public async void UpdateUserName(string username){
        var requestData = new UpdateProfileParams{
            username = username
        };

        var response = await _apiService.SendUpdateProfileRequest(requestData, _userloginData.token);
        if(response.IsSuccess) {
            _sessionUserData = response.SuccessResponse.data;
            OnUserDataUpdate?.Invoke(_sessionUserData);
        }
        else{
            var popup = new MessagePopup{
                header= "Failed to update!!!",
                message= response.FailureResponse.message,
                banner = BannerType.Danger,
                exits = new List<MessagePopupExit>{
                    new MessagePopupExit{
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                        name = "okay",
                        exitAction = () => {}
                    }
                }
            };

            _gameCanvas.ShowMessagePopup(popup);
        }
    }

    //=======================
    // DELETE USER
    //=======================

    public async void DeleteUser(){
        var response = await _apiService.SendDeleteUserRequest(_userloginData.token);
        if(response.IsSuccess) {
            Debug.Log("User, Deleted");
        }
        else{
            var popup = new MessagePopup{
                header= "Failed to Delete!!!",
                message= response.Response.message,
                banner = BannerType.Danger,
                exits = new List<MessagePopupExit>{
                    new MessagePopupExit{
                        exitStyle = MessagePopupExit.ExitStyle.Regular,
                        name = "okay",
                        exitAction = () => {}
                    }
                }
            };

            _gameCanvas.ShowMessagePopup(popup);
        }
    }
}
}
