using TA.Services;
using TA.APIClient;
using TA.APIClient.RequestData;
using TA.UserProfile;
using TA.Components;
using TA.Helpers;
using TA.Menus;
using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using static TA.Components.MessagePopupExit;

namespace TA.Game{
public class GameService : Service<GameService> {
    [SerializeField] bool useTestCredits;
    [SerializeField] int testCredits;

    APIService _apiService;
    UserProfileService _userProfileService;
    BlockchainGameCanvas _blockChainGameCanvas;
    TAMenuService _taMenuService;

    string _gameId;
    string _gameToken = null;
    public bool InSession => _gameToken != null;

    int _totalScore;
    float _duration;
    float _startTime;
    string _timeStamp;

    int _retries;

    public int SavedTotalScore => _totalScore;

    public Action OnStartSessionSuccess;
    public Action OnStartSessionFailed;

    public Action OnUpdateScoreFailed;
    public Action OnUpdateScoreSucess;

    public Action OnContinueGameFailed;
    public Action OnContinueGameSuccess;

    public Action OnEndSessionSuccess;
    public Action OnEndSessionFailed;

    public Action OnExitGameRequest;

    string AuthToken { 
        get{
            if(_userProfileService.UserData == null){
                throw new System.Exception("Could not get auth token, make sure property is invoked only after autherization success");
            }
            return _userProfileService.UserData.token;
        }
    }

    string GameToken {
        get {
            if(_gameToken == null){
                throw new System.Exception("Could not get game token, make sure property is invoked only after game start API has responded with success");
            }
            return _gameToken;
        }
    }

    int Credits {
        get{
            if(useTestCredits) return testCredits;
            else return _userProfileService.UserBalanceData.credits;
        }
    }

    protected override void OnInitialize(){
    }

    void Start(){
        _apiService = ServiceLocator.Instance.GetService<APIService>();
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _blockChainGameCanvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
        _taMenuService = ServiceLocator.Instance.GetService<TAMenuService>();
        _gameId = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig.gameId;
    } 

    void OpenBuyCredit(){
        _taMenuService.OpenInGameCreditShop();
    }

    void ShowErrorMessage(string message, Action action){
        _blockChainGameCanvas.ShowMessagePopup(ShowAPIFailureMessagePopUp(message, action));
    }

    public int NextContinueCost => GetContinueCost(_retries);

    public static int GetContinueCost(int retries) {
        if (retries < 0) {
            throw new ArgumentException("Number of retries cannot be negative.");
        }

        if (retries == 0 || retries == 1) {
            return 1;
        }

        int a = 1;
        int b = 1;

        for (int i = 2; i <= retries; i++) {
            int temp = a + b;
            a = b;
            b = temp;
        }

        return b;
    }

    MessagePopup GetInsufficientFundsPopUp(){
        return new MessagePopup{
            message = "You don't have enough credits to continue playing. Buy more credits to continue",
            header = "Not Enought Credits!",
            banner = BannerType.None,
            hasBackground = false,
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit{
                    name = "Okay"
                },
                new MessagePopupExit{
                    name = "Buy Credits",
                    exitStyle = ExitStyle.Confirmation,
                    exitAction = OpenBuyCredit 
                }
            }
        };
    }

    MessagePopup ShowAPIFailureMessagePopUp(string message, Action action){
        return new MessagePopup{
            message = message,
            header = "Something Went wrong",
            banner = BannerType.Danger,
            hasBackground = false,
            exits = new List<MessagePopupExit>(){
                new MessagePopupExit{
                    name = "Okay",
                    exitStyle = MessagePopupExit.ExitStyle.Confirmation,
                    exitAction = action
                } 
            }
        };
    }

    async UniTask<bool> TryUserBalance(Action onFailed){
        var response = await _userProfileService.UpdateUserBalance();
        if(response.IsSuccess) {
            return true;
        }else{
            ShowErrorMessage("Failed to fetch user balance: " + response.Response.message, onFailed);
            return false;
        }
    }

    //============================
    // API METHODS
    //============================

    public async void StartGameSession(){
        if(InSession) {
            ShowErrorMessage("A Game session is already active", OnStartSessionFailed);
            return;
        }

        if(Credits < 1){
            _blockChainGameCanvas.ShowMessagePopup(GetInsufficientFundsPopUp(), 1);
            return;
        }

        var response = await _apiService.SendStartGameRequest(_gameId, AuthToken);
        if(!response.IsSuccess){
            var result = await TryUserBalance(OnStartSessionFailed);
            if(!result) return;

            _gameToken = response.SuccessResponse.data.token;
            _retries =0;
            _totalScore = 0;
            _duration = Time.time;
            _startTime = Time.time;
            _timeStamp = DataTimeHelper.GetCurrentTimeInIsoFormat();

            OnStartSessionSuccess?.Invoke();
        }
        else{
            ShowErrorMessage(response.FailureResponse.message, OnStartSessionFailed);
        }
    }

    public async void UpdateScore(int score){
        var sessionScore = score - _totalScore; 
        var duration = Time.time - _duration;
        var endStamp = DataTimeHelper.GetCurrentTimeInIsoFormat(); 
        var starStamp = _timeStamp;
 
        var param = new UpdateScoreParams{
            sessionScore = sessionScore.ToString(),
            duration = duration.ToString(),
            startTime = starStamp,
            endTime = endStamp
        };

        var response = await _apiService.SendUpdateGameRequest(param, GameToken);
        if(!response.IsSuccess){
            OnUpdateScoreSucess?.Invoke();
            _totalScore = score;
            _duration = Time.time;
            _timeStamp = DataTimeHelper.GetCurrentTimeInIsoFormat();
        }else{
            ShowErrorMessage(response.FailureResponse.message, OnUpdateScoreFailed);
        }
    }

    public async void ContinueGame(){
        if(Credits < GetContinueCost(_retries)){
            _blockChainGameCanvas.ShowMessagePopup(GetInsufficientFundsPopUp(), 1);
            return;
        }

        var response = await _apiService.SendContinueRequest(AuthToken);
        if(!response.IsSuccess){
            var result = await TryUserBalance(OnContinueGameFailed);
            if(!result) return;

            _gameToken = response.SuccessResponse.data.token;
            _retries++;
 
            OnContinueGameSuccess?.Invoke();
        }
        else{
            ShowErrorMessage(response.FailureResponse.message, OnContinueGameFailed);
        }
    }

    public async void EndGameSession(){
        var param = new FinalScoreParams{
            totalScore = _totalScore.ToString()
        };

        var response = await _apiService.SendCompleteRequest(param, GameToken);
        if(!response.IsSuccess){
            _gameToken = null;
            OnEndSessionSuccess?.Invoke();
        }else{
            ShowErrorMessage(response.FailureResponse.message, OnEndSessionFailed);
        }
    }

    public void MakeExitGameRequest(){
        OnExitGameRequest?.Invoke();
    }
}}
