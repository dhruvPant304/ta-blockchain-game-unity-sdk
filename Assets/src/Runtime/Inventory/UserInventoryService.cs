using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserProfile;
using TA.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TA.UserInventory{
public class UserInventoryService : Service<UserInventoryService>{
    UserProfileService _userProfileService;
    APIService _apiService;
    BlockchainGameCanvas _canvas;

    void Start(){
        _userProfileService = ServiceLocator.Instance.GetService<UserProfileService>();
        _apiService = ServiceLocator.Instance.GetService<APIService>();
        _canvas = ServiceLocator.Instance.GetService<BlockchainGameCanvas>();
    }

    public Action<List<InventoryEntry<IShopItem>>> OnInventoryUpdate;

    public async UniTask<List<InventoryEntry<T>>> GetInventory<T>() where T : class, IShopItem {
        var inventory = await _apiService.SendFetchUserInventoryRequest<object>(_userProfileService.LoginToken); 
        if(!inventory.IsSuccess){
            return null;
        }

        List<InventoryEntry<T>> parsableEntries = new();
        foreach(var entry in inventory.SuccessResponse.data){
            try{
                var itemJson = JsonConvert.SerializeObject(entry.item);
                var converted = JsonConvert.DeserializeObject<T>(itemJson);

                var convertedEntry = new InventoryEntry<T>(){
                    quantity = entry.quantity,
                    lastPurchaseTime = entry.lastPurchaseTime,
                    item = converted
                };

                parsableEntries.Add(convertedEntry);
            }catch{
                continue;
            }
        }
        parsableEntries.Sort((a,b) => a.item.ShopId.CompareTo(b.item.ShopId));
        return parsableEntries;
    }

    public async UniTask RefreshInevntory<T>() where T : class, IShopItem{
        var updated = await GetInventory<T>();
        var converted = updated.Select((entry) => new InventoryEntry<IShopItem> {
                     quantity = entry.quantity,
                     lastPurchaseTime = entry.lastPurchaseTime,
                     item = (IShopItem)entry.item
                }).ToList();
        OnInventoryUpdate?.Invoke(converted);
    }
}
}
