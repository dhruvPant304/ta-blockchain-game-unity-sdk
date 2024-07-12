using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
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

        //=====================
        // GAME 
        //=====================

        public async UniTask<VariableRequestResponse<GameSessionResponse, FailedResponse>> SendStartGameRequest(string gameId,
                string authToken){
            return await SendWebRequest<GameSessionResponse, FailedResponse>(
                        $"/api/v1/games/start/{gameId}",
                        "POST",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<ScoreUpdateResponse , FailedResponse>> SendUpdateGameRequest(
                TA.APIClient.RequestData.UpdateScoreParams updateGameParams,
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
           var json = JsonUtility.ToJson(body);
            if (_config.logRequest)
                Debug.Log($"Sent: {uri} \n body: {json}");
            var data = Encoding.UTF8.GetBytes(json);
            if (body != null) request.uploadHandler = new UploadHandlerRaw(data);
            return request;
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
