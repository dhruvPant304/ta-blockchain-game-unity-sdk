using System;
using System.Collections.Generic;
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

        public async UniTask<VariableRequestResponse<InventoryResponse, FailedResponse>> SendFetchUserInventoryRequest(string authToken){
            return await SendWebRequest<InventoryResponse,FailedResponse>(
                        "/api/v1/user/inventory",
                        "GET",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse, FailedResponse>> SendConsumeShopItemRequest(IShopItem item, string authToken){
            var param = new ConsumeItemParams(){
                id = item.ShopId.ToString()
            };

            return await SendWebRequest<BaseAPIResponse,FailedResponse>(
                        "/api/v1/user/consume-shop-item",
                        "PATCH",
                        param,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse,FailedResponse>> SendUpdateDeviceTokenRequest(
                string authToken, 
                DeviceId details){
            return await SendWebRequest<BaseAPIResponse,FailedResponse>(
                        "/api/v1/user/device",
                        "PATCH",
                        details,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse, FailedResponse>> SendDeleteDevice(string authToken){
            return await SendWebRequest<BaseAPIResponse, FailedResponse>(
                        "/api/v1/user/device",
                        "DELETE",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse, FailedResponse>> SendVerifyDeviceRequest(
                string authToken,
                DeviceDetails details){
            return await SendWebRequest<BaseAPIResponse, FailedResponse>(
                        "/api/v1/user/verify-device-token",
                        "PATCH",
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

        public async UniTask<VariableRequestResponse<CoinEarnedResponse, FailedResponse>> SendAddGameCoinRequest(int coins, string authToken){
            var param = new AddCoinParams(){coinEarned = coins};
            return await SendWebRequest<CoinEarnedResponse,FailedResponse>(
                        "/api/v1/games/earned-coins",
                        "POST",
                        param,
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
                TA.IAP.InitiatePurchaseRequest data, 
                string authToken){
            return await SendWebRequest<InitiatePaymentResponse, FailedResponse>(
                    $"/api/v1/payments/initiate-payment",
                    "POST",
                    data,
                    authToken
                    );
        }

        public async UniTask<VariableRequestResponse<BaseAPIResponse, FailedResponse>> SendVerificationRequest(
                TA.IAP.PurhcaseVerificationRequest data,
                string authToken){
            return await SendWebRequest<BaseAPIResponse, FailedResponse>(
                    $"/api/v1/payments/verify-payment",
                    "POST",
                    data,
                    authToken
                    );
        }

        //=====================
        // SHOP
        //=====================

        public async UniTask<VariableRequestResponse<BoosterResponse<T>, FailedResponse>> SendFetchBoostersRequest<T>() 
            where T : class, IShopItem{
            return await SendWebRequest<BoosterResponse<T>, FailedResponse>(
                    $"/api/v1/shop/boosters",
                    "GET"
                    );
        }

        public async UniTask<VariableRequestResponse<CRUDDBData, FailedResponse>> SendBuyItemRequest(IShopItem item, 
                int quantity, 
                string authToken){
            var param = new BuyItemParams(){
                itemId = item.ShopId,
                quantity = quantity,
                itemType = item.ItemType
            };

            return await SendWebRequest<CRUDDBData, FailedResponse>(
                    $"/api/v1/shop/item/buy",
                    "POST",
                    param,
                    authToken
            );
        }

        public async UniTask<VariableRequestResponse<CheckFreeBoosterResponse,FailedResponse>> SendCheckFreeBoosterAvailableRequest(
                string authToken){
            return await SendWebRequest<CheckFreeBoosterResponse,FailedResponse>(
                        $"/api/v1/shop/claimed-free-booster",
                        "GET",
                        null,
                        authToken
                    );
        }

        public async UniTask<VariableRequestResponse<APIResponse<CRUDDBData>,FailedResponse>> SendClaimFreeBoosterRequest(string authToken){
            return await SendWebRequest<APIResponse<CRUDDBData>, FailedResponse>(
                        $"/api/v1/shop/claim-free-booster",
                        "POST",
                        null,
                        authToken
                    );
        }

        //=====================
        // PUSH NOTIFICATIONS
        //=====================

        

        //=====================
        // METHODS
        //=====================

        private async UniTask<VariableRequestResponse<TSuccess, TFailure>> SendWebRequest<TSuccess,TFailure>(string relativeAPIPath, string method, object param = null, string authToken = ""){
            var absoluteUrl = _config.serverUrl + relativeAPIPath;
            var request = new DetailedWebRequest(_config);
            request.Construct(absoluteUrl, method, param, authToken);
            var res = await SendWebRequest<TSuccess,TFailure>(request);
            request.LogRequest();
            return res;
        }

        private async UniTask<VariableRequestResponse<TSuccess, TFailure>> SendWebRequest<TSuccess,TFailure>(DetailedWebRequest request){
            var response = new VariableRequestResponse<TSuccess, TFailure>();
            try{ 
                await request.webRequest.SendWebRequest();
            }
            catch(Exception e){
                Debug.LogError($"Error while sending request: {e}");
            }
            finally{
                var responseText = request.webRequest.downloadHandler.text;
                request.response = responseText;

                if (request.webRequest.result == UnityWebRequest.Result.Success){
                    request.isSuccess = true;
                    response.IsSuccess = true;
                    try{
                        response.SuccessResponse = JsonConvert.DeserializeObject<TSuccess>(responseText);
                    }catch(Exception e){
                        throw new Exception($"API response parsing Error:{e} \n Recieved: {responseText}");
                    }
                }
                else{
                    request.isSuccess = true;
                    response.IsSuccess = false;
                    try{
                        response.FailureResponse = JsonConvert.DeserializeObject<TFailure>(responseText);
                    }catch(Exception e){
                        throw new Exception($"API response parsing Error:{e} \n Recieved: {responseText}");
                    }
                }
            }
            return response;
        }

        private async UniTask<StaticRequestResponse<TResponse>> SendWebRequest<TResponse>(string relativePath, string method, object param = null, string authToken = ""){
            var variableResponse = await SendWebRequest<TResponse,TResponse>(relativePath, method, param, authToken);
            return new StaticRequestResponse<TResponse>(variableResponse);
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
    public class DetailedWebRequest{
        public UnityWebRequest webRequest;
        public string url;
        public string body;
        public string response;
        public bool isSuccess = false;
        public Dictionary<string,string> headers = new();
        private APIConfig _config;

        public DetailedWebRequest(APIConfig config){
            _config = config;
        }

        public void Construct(string uri, string method, object body = null, string authToken = ""){
            url = $"{uri}";
            webRequest = CreateRequest(uri,method,body,authToken);
        }


        private UnityWebRequest CreateRequest(string uri, string method, object body = null, string authToken = ""){
            var request = new UnityWebRequest(uri);
            request.method = method;

            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(authToken)) {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
                headers["Authorization"] = $"Bearer {authToken}";
            }

            headers["Accept"] = "application/json";
            headers["Content-Type"] = "application/json";

            request.downloadHandler = new DownloadHandlerBuffer();
            request.disposeUploadHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;

            var json = JsonConvert.SerializeObject(body);
            this.body = json;
            var data = Encoding.UTF8.GetBytes(json);
            if (body != null) request.uploadHandler = new UploadHandlerRaw(data);
            return request;
        }

        public void LogRequest(){
            var msg = $"URL: {url}\n HEADERS: [";
            foreach(var header in headers){
                msg += $"{header.Key} : {header.Value},";
            }
            msg += "]";
            msg += $"\nPAYLOAD: {body}";
            msg += $"\nRESPONSE: {response}";

            if(isSuccess){
                Debug.Log(msg);
            }else{
                Debug.LogError(msg);
            }
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

