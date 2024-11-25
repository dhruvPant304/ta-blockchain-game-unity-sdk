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
using TA.Leaderboard;
using System.Globalization;

namespace TA.Game{
public class GameService : Service<GameService> {
    [SerializeField] bool useTestCredits;
    [SerializeField] int testCredits;

    APIService _apiService;
    UserProfileService _userProfileService;
    BlockchainGameCanvas _blockChainGameCanvas;
    TAMenuService _taMenuService;
    LeaderboardService _leaderBoardService;
    APIConfig _apiConfig;

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
            if(_userProfileService.LoginUserData == null){
                throw new System.Exception("Could not get auth token, make sure property is invoked only after autherization success");
            }
            return _userProfileService.LoginUserData.token;
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
        _apiConfig = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;


        StartCoroutine(ExecuteUpdateRequestQueue().ToCoroutine());
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

    //============================
    // START GAME 
    //============================

    public async void StartGameSession(){
        if(InSession) {
            ShowErrorMessage("A Game session is already active", OnStartSessionFailed);
            return;
        }

        if(_apiConfig.requireCreditsToPlay){
            if(Credits < 1){
                _blockChainGameCanvas.ShowMessagePopup(GetInsufficientFundsPopUp(), 1);
                return;
            }
        }

        var startParams = new StartGameParams(){
            isFreeToPlay = !_apiConfig.requireCreditsToPlay
        };

        var response = await _apiService.SendStartGameRequest(_gameId, startParams, AuthToken);
        if(response.IsSuccess){
            var result = await TryUserBalance(OnStartSessionFailed);
            if(!result) return;

            _gameToken = response.SuccessResponse.data.token;
            _retries =0;
            _totalScore = 0;
            _duration = Time.time;
            _startTime = Time.time;
            _timeStamp = DataTimeHelper.GetCurrentTimeInIsoFormat();

            OnStartSessionSuccess?.Invoke();
            updateRequestBuffer = new(DateTime.Parse(_timeStamp));
            Debug.Log($"Starting Game session with token: {_gameToken}");
        }
        else{
            ShowErrorMessage(response.FailureResponse.message, OnStartSessionFailed);
        }
    }

    //============================
    // UPDATE GAME - BEGIN
    //============================
    
    UpdateRequestBuffer updateRequestBuffer = null;

    public void UpdateScore(int score){
        if(updateRequestBuffer == null){
            throw new Exception("Failed to update score: Update request buffer is null");
        }

        UpdateScoreRequest request = null;
        int sessionScore = score - updateRequestBuffer.ActiveScore;
        var starStamp = updateRequestBuffer.LastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        var endStamp = DataTimeHelper.GetCurrentTimeInIsoFormat();
        var duration = (DateTime.Parse(starStamp) - DateTime.Parse(endStamp)).ToString();

        request = new UpdateScoreRequest{
            sessionScore = sessionScore.ToString(),
            duration = duration.ToString(),
            startTime = starStamp,
            endTime = endStamp
        };

        var queuedRequest = new UpdateScoreQueuedRequest{
            request = request,
            addedScore = sessionScore
        };

        updateRequestBuffer.Add(queuedRequest);
        if(_apiConfig.bufferUpdateScoreRequest) updateRequestBuffer.Compress(_apiConfig.updateScoreBufferDuration);
    }

    async UniTask ExecuteUpdateRequest(UpdateScoreQueuedRequest queuedRequest){
        var response = await _apiService.SendUpdateGameRequest(queuedRequest.request, GameToken);
        if(response.IsSuccess){
            _totalScore += queuedRequest.addedScore;
            _duration = Time.time;
            _timeStamp = DataTimeHelper.GetCurrentTimeInIsoFormat();

            OnUpdateScoreSucess?.Invoke();
        }else{
            ShowErrorMessage(response.FailureResponse.message, OnUpdateScoreFailed);
        }
    }

    async UniTask ExecuteUpdateRequestQueue(){
        while(true){
            await UniTask.WaitUntil(() => updateRequestBuffer.HasAny);
            var curr = updateRequestBuffer.Peek;
            updateRequestBuffer.Pop();
            await ExecuteUpdateRequest(curr);
        }
    }

    public class UpdateRequestBuffer{
        List<UpdateScoreQueuedRequest> queue = new();
        public int ActiveScore {get; private set;} = 0;
        public DateTime LastUpdateTime {get; private set;}

        public UpdateRequestBuffer(DateTime startTime){
            LastUpdateTime =startTime;
        }

        public void Add(UpdateScoreQueuedRequest item){
            queue.Add(item);
            ActiveScore += item.addedScore;
            queue.Sort((a,b) => {
                var startA = DateTime.Parse(a.request.startTime); 
                var startB = DateTime.Parse(b.request.startTime);
                return startA.CompareTo(startB);
            });
            LastUpdateTime = DateTime.Parse(Last.request.endTime);
        }

        public void Pop(){
            if(queue.Count == 0) return;
            queue.RemoveAt(0);
        }

        public void Compress(float duration){
            if(queue.Count == 0) return;
            List<UpdateScoreQueuedRequest> compressedList = new();
            compressedList.Add(queue[0]);
            for(int i =1 ; i < queue.Count; i++){
                var last = compressedList.Count - 1;
                var start = DateTime.Parse(compressedList[last].request.startTime);
                var current = DateTime.Parse(queue[i].request.endTime);

                if((current - start).TotalSeconds < duration){
                    compressedList[last].request.sessionScore += queue[i].request.sessionScore;
                    compressedList[last].request.endTime = queue[i].request.endTime;
                    compressedList[last].request.duration += queue[i].request.duration;
                    continue;
                }
                compressedList.Add(queue[i]);
            }

            queue = compressedList;
        }
        public UpdateScoreQueuedRequest Peek => queue[0];
        public UpdateScoreQueuedRequest Last => queue[queue.Count - 1];
        public bool HasAny => queue.Count > 0;
    }

    public class UpdateScoreQueuedRequest{
        public UpdateScoreRequest request;
        public int addedScore;
    }

    //============================
    // UPDATE GAME - END
    //============================

    public async UniTask<int> FetchCurrentUserRank(){
        var activeLeaderBoard = await _leaderBoardService.GetActiveHighScoreLeaderBoard();
        var userStats = await activeLeaderBoard.GetUserStats();
        return userStats.rank;
    }

    public async void ContinueGame(){
        if(_apiConfig.requireCreditsToPlay){
            if(Credits < GetContinueCost(_retries)){
                _blockChainGameCanvas.ShowMessagePopup(GetInsufficientFundsPopUp(), 1);
                return;
            }
        }

        var response = await _apiService.SendContinueRequest(GameToken);
        if(response.IsSuccess){
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

    public void EndGameSession(){
        var param = new FinalScoreParams{
            totalScore = _totalScore.ToString()
        };

        _gameToken = null;
        OnEndSessionSuccess?.Invoke();
        //var response = await _apiService.SendCompleteRequest(param, GameToken);
        // if(response.IsSuccess){
        // }else{
        //     //ShowErrorMessage(response.FailureResponse.message, OnEndSessionFailed);
        // }
    }

    public void MakeExitGameRequest(){
        OnExitGameRequest?.Invoke();
    }
}}
