using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserProfile;

namespace TA.InGameShop{
public interface IShop<T> where T : IShopItem {
    public UniTask<List<T>> GetAllShopItems();
    public UniTask<VariableRequestResponse<CRUDDBData, FailedResponse>> Buy(T item, int quantity);
    public UniTask<bool> CheckFreeItemAvailable();
    public UniTask<bool> ClaimFreeItem();
}

public abstract class Shop<T> : IShop<T> where T : IShopItem{ 
    public abstract UniTask<List<T>> GetAllShopItems();
    public async UniTask<VariableRequestResponse<CRUDDBData, FailedResponse>> Buy(T item, int quantity){
        var api = ServiceLocator.Instance.GetService<APIService>();
        var profile = ServiceLocator.Instance.GetService<UserProfileService>();
        var token = profile.LoginToken;
        var res = await api.SendBuyBoosterRequest(item,quantity,token);
        await profile.UpdateUserBalance();
        return res;
    }
    public abstract UniTask<bool> CheckFreeItemAvailable();
    public abstract UniTask<bool> ClaimFreeItem();
}
}
