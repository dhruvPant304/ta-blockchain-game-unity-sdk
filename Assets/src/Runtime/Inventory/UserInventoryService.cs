using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserProfile;
using TA.Components;
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

    public Action<List<InventoryEntry>> OnInventoryUpdate;
    public Dictionary<int, InventoryEntry> _inventoryCache = new();

    public InventoryCollection GetInventory() {
        var cached = _inventoryCache
            .Select(pair => pair.Value)
            .ToList();

        var filteredCollection = new InventoryCollection();
        filteredCollection.AddRange(cached);

        return filteredCollection;
    }

    public void AddToInventoryCached<T>(T item, int amount) where T : class, IShopItem{
        if(_inventoryCache.ContainsKey(item.ShopId) == false){
            throw new Exception($"Cannot find entry with id: \"{item.ShopId}\" in inventory cache, make sure you call SyncInventory atleast once before");
        }
        _inventoryCache[item.ShopId].quantity += amount;
        var callbackParams = _inventoryCache.Select((pair) => pair.Value).ToList();
        OnInventoryUpdate?.Invoke(callbackParams);
        FetchInventory().Forget();
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
    
    public async UniTask<InventoryCollection> FetchInventory(){
        var collection  = new InventoryCollection();
        var res = await _apiService.SendFetchUserInventoryRequest(_userProfileService.LoginToken); 
        if(!res.IsSuccess){
            return collection;
        }
        var entries = res.SuccessResponse.data.ToList();

        entries.ForEach((ent) => {
            var temp = ent.Parse<ItemTemp>();
            _inventoryCache[temp.item.id] = ent;
        });

        collection.AddRange(entries);
        return collection;
    }

    async UniTask<bool> SendConsumptionRequest<T>(T item) where T : class, IShopItem {
        var res = await _apiService.SendConsumeShopItemRequest(item, _userProfileService.LoginToken); 
        await FetchInventory();
        return res.IsSuccess;
    }
}

public class ItemTemp{
    public int id;
    public string itemType;
}

public class InventoryCollection : List<InventoryEntry> { 
    public List<InventoryEntry<T>> As<T>() where T: class, IShopItem{
        List<InventoryEntry<T>> list = new();
        return this
            .Where(e => {
                var success = false;
                e.TryParse<T>(ref success);
                return success;
            })
            .Select(e => e.Parse<T>()).ToList();
    }
}
}
