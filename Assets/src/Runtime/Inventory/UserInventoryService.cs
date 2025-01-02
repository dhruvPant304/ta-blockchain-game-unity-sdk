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
    public Dictionary<int, InventoryEntry<IShopItem>> _inventoryCache = new();

    public List<InventoryEntry<T>> GetInventory<T>(string type) where T: class, IShopItem{
        var filtered = _inventoryCache
            .Select((pair) => pair.Value)
            .Where((entry) => entry.item.ItemType == type)
            .Select((entry) => new InventoryEntry<T>{
                quantity = entry.quantity,
                item = (T)entry.item
            }).ToList();

        return filtered;
    }

    public void AddToInventoryCached<T>(T item, int amount) where T : class, IShopItem{
        if(_inventoryCache.ContainsKey(item.ShopId) == false){
            throw new Exception($"Cannot find entry with id: \"{item.ShopId}\" in inventory cache, make sure you call SyncInventory atleast once before");
        }
        _inventoryCache[item.ShopId].quantity += amount;
        var callbackParams = _inventoryCache.Select((pair) => pair.Value).ToList();
        OnInventoryUpdate?.Invoke(callbackParams);
        FetchInventory<T>(item.ItemType).Forget();
    }

    public void TakeFromInventoryCached<T>(T item) where T : class, IShopItem{
        if(_inventoryCache.ContainsKey(item.ShopId) == false){
            throw new Exception($"Cannot find entry with id: \"{item.ShopId}\" in inventory cache, make sure you call SyncInventory atleast once before");
        }
        if(_inventoryCache[item.ShopId].quantity < 1){
            throw new Exception($"Should have atleast one in inventory to consume");
        }
        _inventoryCache[item.ShopId].quantity += 1;
        var callbackParams = _inventoryCache.Select((pair) => pair.Value).ToList();
        OnInventoryUpdate?.Invoke(callbackParams);
        SendConsumptionRequest<T>(item).Forget();
    }

    private async UniTask<List<InventoryEntry<T>>> FetchInventory<T>(string type) where T : class, IShopItem {
        var inventory = await _apiService.SendFetchUserInventoryRequest<object>(_userProfileService.LoginToken); 
        if(!inventory.IsSuccess){
            return null;
        }

        List<InventoryEntry<T>> filteredByReqType = new();
        List<InventoryEntry<IShopItem>> callBackParams = new();
        foreach(var entry in inventory.SuccessResponse.data){
            try{
                var itemJson = JsonConvert.SerializeObject(entry.item);
                var converted = JsonConvert.DeserializeObject<T>(itemJson);

                if(converted.ItemType != type) continue;

                var convertedEntry = new InventoryEntry<T>(){
                    quantity = entry.quantity,
                    lastPurchaseTime = entry.lastPurchaseTime,
                    item = converted
                };

                var abstractEntry = new InventoryEntry<IShopItem>(){
                    quantity = entry.quantity,
                    lastPurchaseTime = entry.lastPurchaseTime,
                    item = converted
                };

                filteredByReqType.Add(convertedEntry);
                callBackParams.Add(abstractEntry);
                _inventoryCache[converted.ShopId] = abstractEntry;
            }catch{
                continue;
            }
        }
        filteredByReqType.Sort((a,b) => a.item.ShopId.CompareTo(b.item.ShopId));
        return filteredByReqType;
    }

    public async UniTask InitInventory<T>(string type) where T : class, IShopItem {
        var fetched = await FetchInventory<T>(type);
        var args = fetched.Select((ent) => {
            return new InventoryEntry<IShopItem>(){
                quantity = ent.quantity,
                lastPurchaseTime = ent.lastPurchaseTime,
                item = ent.item
            };
        }).ToList();
        OnInventoryUpdate?.Invoke(args);

    }

    async UniTask<bool> SendConsumptionRequest<T>(T item) where T : class, IShopItem {
        var res = await _apiService.SendConsumeShopItemRequest(item, _userProfileService.LoginToken); 
        await FetchInventory<T>(item.ItemType);
        return res.IsSuccess;
    }
}
}
