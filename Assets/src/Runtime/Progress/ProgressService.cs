using Cysharp.Threading.Tasks;
using TA.Services;
using TA.UserProfile;
using TA.APIClient;
using TA.APIClient.RequestData;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TA.Progress{
public class ProgressController<T> where T : struct{
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
        controller.DefineSchema(schema);
        return controller;
    }

    InventoryObject<T> inventory;
    T defaultLoadData;

    void DefineSchema(T schema){
        inventory.Set(schema);
        defaultLoadData = schema;
    }

    public async UniTask SaveProgress(T updatedData){
        await WaitLogin();
        inventory.Set(updatedData);
        await _apiService.SendUpdateGameProgressRequest(inventory.GetUpdateParams(), _userProfileService.LoginToken);
    }

    public T GetDefaultValues(){
        return defaultLoadData;
    }

    public async UniTask<T?> LoadProgress(){
        await WaitLogin();
        var res = await _apiService.SendFetchGameProgressRequest(_userProfileService.LoginToken); 
        if(res.IsSuccess){
            inventory.Set(res.SuccessResponse.data.progress);
            return inventory.Get();
        }else{
            return null;
        }
    }

    async UniTask WaitLogin(){
        await UniTask.WaitUntil(() => _userProfileService.LoggedIn);
    }
}

public class InventoryObject<T> {
    JObject data;
   
    public void Set(JObject jobject){
        data = jobject;
    }

    public void Set(T defaultValue){
        data = new JObject(defaultValue);
    }

    public T Get(){
        return JsonConvert.DeserializeObject<T>(data.ToString());
    }

    public ProgressParams GetUpdateParams(){
        return new ProgressParams(){
            progress = data
        };
    }
}
}

