using Cysharp.Threading.Tasks;
using TA.Services;
using TA.UserProfile;
using TA.APIClient;
using TA.APIClient.RequestData;
using UnityEngine;

namespace TA.Progress{
public class ProgressController<T> where T : class{
    UserProfileService _userProfileService;
    APIService _apiService;

    public int UserLevel {get; private set;}

    ProgressController(UserProfileService profile, APIService api){
        _userProfileService = profile;
        _apiService = api;
    }

    public static ProgressController<T> Create(T schema){
        var profile = ServiceLocator.Instance.GetService<UserProfileService>();
        var api = ServiceLocator.Instance.GetService<APIService>();
        var controller = new ProgressController<T>(profile,api);
        return controller;
    }

    T defaultLoadData;

    public async UniTask SaveProgress(T updatedData){
        await WaitLogin();
        var progressParam = new ProgressParams{
            progress = updatedData
        };
        await _apiService.SendUpdateGameProgressRequest<T>(progressParam, _userProfileService.LoginToken);
    }

    public T GetDefaultValues(){
        return defaultLoadData;
    }

    public async UniTask<T> LoadProgress(){
        await WaitLogin();
        var res = await _apiService.SendFetchGameProgressRequest<T>(_userProfileService.LoginToken); 
        if(res.IsSuccess){
            try{
                UserLevel = res.SuccessResponse.data.userLevel;
                return res.SuccessResponse.data.progress;
            }
            catch(System.Exception e){
                Debug.LogError($"Error while fetchin user save data: {e}");
                return null;
            }
        }else{
            return null;
        }
    }

    async UniTask WaitLogin(){
        await UniTask.WaitUntil(() => _userProfileService.LoggedIn);
    }
}
}

