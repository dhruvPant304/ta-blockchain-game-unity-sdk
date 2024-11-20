using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TA.APIClient.RequestData;
using TA.APIClient.ResponseData;
using TA.Services;
using UnityEngine;
using UnityEngine.Networking;

namespace TA.APIClient{
    public class APIService : Service<APIService>{

        APIConfig _config;
        void Start(){
            _config = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;

            Debug.Log($"API client connected to server: {_config.serverUrl}");
        }

        protected override void OnInitialize(){
        }

        //=====================
        // AUTHORIZATION
        //=====================

        public async UniTask<VariableRequestResponse<LoginResponse,FailedResponse>> SendLoginRequest(
                TA.APIClient.RequestData.LoginParams loginParams){
            return await SendWebRequest<LoginResponse,FailedResponse>(
                        "/api/v1/auth/login",
                        "POST",
                        loginParams
                    );
        }

        //=====================
        // USER 
        //=====================

        public async UniTask<StaticRequestResponse<UserBalanceResponse>> SendFetchUserBalanceRequest(string authToken){
            return await SendWebRequest<UserBalanceResponse>(
                        "/api/v1/user/balance",
                        "GET",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<UserDataResponse, FailedResponse>> SendUpdateProfileRequest(UpdateProfileParams updatedUserData, string authToken){
            return await SendWebRequest<UserDataResponse, FailedResponse>(
                        "/api/v1/user/profile",
                        "PUT",
                        updatedUserData,
                        authToken
                    );
        }

        public async UniTask<StaticRequestResponse<BaseAPIResponse>> SendDeleteUserRequest(string authToken){
            return await SendWebRequest<BaseAPIResponse>(
                        "/api/v1/user",
                        "DELETE",
                        null,
                        authToken
                    );
        }

        //=====================
        // SETTINGS
        //=====================

        public async UniTask<VariableRequestResponse<UserDataResponse, FailedResponse>> SendUpdateUserSettingsRequest(AppSettings settings, string authToken){
            return await SendWebRequest<UserDataResponse, FailedResponse>(
                        "/api/v1/user/settings",
                        "PUT",
                        settings,
                        authToken
                    );
        }

        //=====================
        // GAME 
        //=====================

        public async UniTask<VariableRequestResponse<GameSessionResponse, FailedResponse>> SendStartGameRequest(string gameId,
                StartGameParams startParams, string authToken){
            return await SendWebRequest<GameSessionResponse, FailedResponse>(
                        $"/api/v1/games/start/{gameId}",
                        "POST",
                        startParams,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<ScoreUpdateResponse , FailedResponse>> SendUpdateGameRequest(
                TA.APIClient.RequestData.UpdateScoreRequest updateGameParams,
                string gameToken){
            return await SendWebRequest<ScoreUpdateResponse, FailedResponse>(
                        "/api/v1/games/update",
                        "PATCH",
                        updateGameParams,
                        gameToken
                    );
        }

        public async UniTask<VariableRequestResponse<GameSessionResponse, FailedResponse>> SendContinueRequest(string gameToken){
            return await SendWebRequest<GameSessionResponse, FailedResponse>(
                        "/api/v1/games/continue",
                        "PUT",
                        null,
                        gameToken
                    );
        }

        public async UniTask<VariableRequestResponse<GameSessionResponse, FailedResponse>> SendCompleteRequest(
                TA.APIClient.RequestData.FinalScoreParams finalScoreParams,
                string gameToken){
            return await SendWebRequest<GameSessionResponse, FailedResponse>(
                        "/api/v1/games/complete",
                        "PUT",
                        finalScoreParams,
                        gameToken
                    );
        }

        public async UniTask<VariableRequestResponse<ProgressResponse<T>, FailedResponse>> SendFetchGameProgressRequest<T>
            (string authToken)
            where T : class{
            return await SendWebRequest<ProgressResponse<T>, FailedResponse>(
                        "/api/v1/games/progress",
                        "GET",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<ProgressResponse<T>, FailedResponse>> SendUpdateGameProgressRequest<T>  
            (ProgressParams progress, string authToken)
            where T: class{
            return await SendWebRequest<ProgressResponse<T>, FailedResponse>(
                        "/api/v1/games/progress",
                        "PATCH",
                        progress,
                        authToken
                    );
        }

        //=====================
        // LEADER BOARDS 
        //=====================

        public async UniTask<VariableRequestResponse<MasterLeaderboardResponse, FailedResponse>> SendFetchMasterLeaderboardRequest(string gameId){
            return await SendWebRequest<MasterLeaderboardResponse, FailedResponse>(
                        $"/api/v1/leaderboard/{gameId}",
                        "GET",
                        null,
                        null
                    );
        }

        public async UniTask<VariableRequestResponse<LeaderBoardResponse, FailedResponse>> SendFetchLeaderBoardResponse(
                string gameId, 
                string leaderboardId,
                string type,
                int pageLimit, 
                int page) {
            return await SendWebRequest<LeaderBoardResponse, FailedResponse>(
                        $"/api/v1/leaderboard/{gameId}/{leaderboardId}?type={type}&limit={pageLimit}&page={page}",
                        "GET",
                        null,
                        null
                    );
        }

        public async UniTask<VariableRequestResponse<UserLeaderBoardStatsResponse, FailedResponse>> SendFetchUserLeaderBoardStatsRequest(
                string gameId,
                string leaderboardId,
                string type,
                string authToken
                ) {
            return await SendWebRequest<UserLeaderBoardStatsResponse, FailedResponse>(
                    $"/api/v1/leaderboard/user-stats/{gameId}/{leaderboardId}?type={type}",
                    "GET",
                    null,
                    authToken
                    );
        }

        //=====================
        // IN APP PURCHASE 
        //=====================

        public async UniTask<VariableRequestResponse<InitiatePaymentResponse, FailedResponse>> SendInitiatePaymentRequest(
                TA.IAP.InitiatePurchaseData data, 
                string authToken){
            return await SendWebRequest<InitiatePaymentResponse, FailedResponse>(
                    $"/api/v1/payments/initiate-payment",
                    "POST",
                    data,
                    authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse, FailedResponse>> SendVerificationRequest(
                TA.IAP.PurhcaseVerificationData data,
                string authToken){
            return await SendWebRequest<BaseAPIResponse, FailedResponse>(
                    $"/api/v1/payments/verify-payment",
                    "POST",
                    data,
                    authToken
                    );
        }


        //=====================
        // END
        //=====================


        ///<summary>
        ///Use this method for handling API requests that have variable
        ///Success and failure response structures, if the request response
        ///is static the overload SendWebRequest<TSuccess> is preferred
        ///</summary>
        ///<param name="relativeAPIPath"> relativeAPIPath is the API path after the server relativeAPIPath</paramm>
        private async UniTask<VariableRequestResponse<TSuccess, TFailure>> SendWebRequest<TSuccess,TFailure>(string relativeAPIPath, string method, object param = null, string authToken = ""){
            var absoluteUrl = _config.serverUrl + relativeAPIPath;
            var request = CreateRequest(absoluteUrl, method, param, authToken);
            var response = new VariableRequestResponse<TSuccess, TFailure>();
            try{ 
                await request.SendWebRequest();
            }
            catch(Exception e){
                Debug.LogError($"Error while sending request: {e}");
            }
            finally{
                var responseText = request.downloadHandler.text;
                if(_config.logResponses) Debug.Log($"Received: {responseText}");

                if (request.result == UnityWebRequest.Result.Success){
                    response.IsSuccess = true;
                    response.SuccessResponse = JsonConvert.DeserializeObject<TSuccess>(responseText);
                }
                else{
                    response.IsSuccess = false;
                    response.FailureResponse = JsonConvert.DeserializeObject<TFailure>(responseText);
                }
            }
            return response;
        }

        ///<summary>
        ///Use this method for handling API requests that have a static response structure
        ///in case response structure can vary for success and failure use the overload
        ///SendWebRequest<TSuccess,TFailure> which returns a VariableRequestResponse
        ///</summary>
        ///<param name="relativePath"> relativeAPIPath is the API path after the server relativeAPIPath</paramm>
    
        private async UniTask<StaticRequestResponse<TResponse>> SendWebRequest<TResponse>(string relativePath, string method, object param = null, string authToken = ""){
            var variableResponse = await SendWebRequest<TResponse,TResponse>(relativePath, method, param, authToken);
            return new StaticRequestResponse<TResponse>(variableResponse);
        }

        private UnityWebRequest CreateRequest(string uri, string method, object body = null, string authToken = ""){
            var request = new UnityWebRequest(uri);
            request.method = method;
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            if (!string.IsNullOrEmpty(authToken)) {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }
           var json = JsonConvert.SerializeObject(body);
            if (_config.logRequest)
                Debug.Log($"Sent: [{method}] {uri} \nAuthToken: {authToken}\n body: {json}");
            var data = Encoding.UTF8.GetBytes(json);
            if (body != null) request.uploadHandler = new UploadHandlerRaw(data);
            return request;
        }

        public static StaticRequestResponse<BaseAPIResponse> CreateBaseResponse(bool isSuccess, string message){
            VariableRequestResponse<BaseAPIResponse,BaseAPIResponse> variableResponse = new VariableRequestResponse<BaseAPIResponse,BaseAPIResponse>(){
                IsSuccess = isSuccess,
                SuccessResponse = new BaseAPIResponse(){
                    status = isSuccess? "SUCCESS" : "FAILED",
                    message = message
                }
            };

            return new StaticRequestResponse<BaseAPIResponse>(variableResponse);
        }
    }

    [Serializable]
    public class VariableRequestResponse<TSuccess, TFailure>{
        public bool IsSuccess;
        public TSuccess SuccessResponse;
        public TFailure FailureResponse;
    }

    [Serializable]
    public class StaticRequestResponse<TResponse>{
        public bool IsSuccess;
        public TResponse Response;

        public StaticRequestResponse(VariableRequestResponse<TResponse,TResponse> variableRequestResponse){
            IsSuccess = variableRequestResponse.IsSuccess;
            if(IsSuccess) Response = variableRequestResponse.SuccessResponse;
            else Response = variableRequestResponse.FailureResponse;
        }
    }
}
