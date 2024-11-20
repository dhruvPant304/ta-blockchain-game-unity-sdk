using Cysharp.Threading.Tasks;
using TA.Services;
using TA.UserProfile;
using TA.APIClient;
using TA.APIClient.RequestData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TA.Progress{
public class ProgressController<T> where T : class{
    UserProfileService _userProfileService;
    APIService _apiService;

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
        await _apiService.SendUpdateGameProgressRequest(progressParam, _userProfileService.LoginToken);
    }

    public T GetDefaultValues(){
        return defaultLoadData;
    }

    public async UniTask<T> LoadProgress(){
        await WaitLogin();
        var res = await _apiService.SendFetchGameProgressRequest(_userProfileService.LoginToken); 
        if(res.IsSuccess){
            try{
                return JsonConvert.DeserializeObject<T>(res.SuccessResponse.data.progress.ToString());
            }
            catch{
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

