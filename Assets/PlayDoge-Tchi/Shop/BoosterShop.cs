using TA.InGameShop;
using System;
using System.Collections.Generic;
using TA.Services;
using TA.APIClient;
using Cysharp.Threading.Tasks;
using TA.UserProfile;
using TA.UserInventory;

public class BoosterShop : Shop<Booster> {
    APIService _api;
    UserProfileService _profile;
    UserInventoryService _inventory;

    public BoosterShop(APIService api, UserProfileService profile, UserInventoryService invetory){
        _profile = profile; 
        _api = api; 
        _inventory = invetory; 
    }

    public override async UniTask<List<Booster>> GetAllShopItems(){
        var api = ServiceLocator.Instance.GetService<APIService>();
        var response = await api.SendFetchBoostersRequest<Booster>();
        if(response.IsSuccess){
            var boosters = new List<Booster>();
            boosters.AddRange(response.SuccessResponse.data.boosters);
            var freeBooster = response.SuccessResponse.data.freeBooster;
            freeBooster.isFree = true;
            boosters.Add(freeBooster);
            return boosters;
        }else{
            throw new Exception("Failed to fetch booster shop");
        }
    }

    public override async UniTask<bool> CheckFreeItemAvailable(){
        var response = await _api.SendCheckFreeBoosterAvailableRequest(_profile.LoginToken);
        if (response.IsSuccess == false) return false;
        return !response.SuccessResponse.data.isClaimed;
    }

    public override async UniTask<bool> ClaimFreeItem(){
        var response = await _api.SendClaimFreeBoosterRequest(_profile.LoginToken);
        if (response.IsSuccess == false) return false;
        return response.SuccessResponse.IsSuccess;
    }

    public override async UniTask<int> GetNextFreeItemRefreshTimeInSeconds() {
        var response = await _api.SendFetchBoostersRequest<Booster>();
        var renewalTime = float.Parse(response.SuccessResponse.data.boosterRenewTimeInMinutes) * 60f;
        return (int)MathF.Ceiling(renewalTime);
    }
}

[Serializable]
public class Booster : IShopItem {
    public static string BOOSTER_ITEM_TYPE = "boosters";

    public int id;
    public string name;
    public string catagory;
    public string itemtype;
    public int coinprice;
    public bool isFree = false;
    public BoosterMetaData metadata;


    public int ShopId => id; 
    public string DisplayName => name; 
    public string Description => metadata.description;
    public int Price => coinprice;
    public String Currency => "gameCoin";
    public string ItemType => itemtype; 
    public bool IsFree => isFree; 
}

[Serializable]
public class BoosterMetaData{
    public string description;
}
