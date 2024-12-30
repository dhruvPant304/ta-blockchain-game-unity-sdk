using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.Services;
using TA.UserInventory;
using TA.UserProfile;
using TA.UserProfile.Balance;

namespace TA.InGameShop{
public interface IShop<T> where T : IShopItem {
    public UniTask<List<T>> GetAllShopItems();
    public UniTask<bool> Buy(T item, int quantity);
    public UniTask<bool> CheckFreeItemAvailable();
    public UniTask<bool> ClaimFreeItem();
    public UniTask<bool> CanBuy(T item);
    public abstract UniTask<int> GetNextFreeItemRefreshTimeInSeconds();
}

public abstract class Shop<T> : IShop<T> where T : class, IShopItem{ 
    public abstract UniTask<List<T>> GetAllShopItems();
    public async UniTask<bool> Buy(T item, int quantity){
        var api = ServiceLocator.Instance.GetService<APIService>();
        var balance = ServiceLocator.Instance.GetService<UserBalanceService>();
        var profile = ServiceLocator.Instance.GetService<UserProfileService>();
        var inventory = ServiceLocator.Instance.GetService<UserInventoryService>();
        var token = profile.LoginToken;
        var res = await api.SendBuyBoosterRequest(item,quantity,token);
        await balance.UpdateUserBalance();
        await inventory.RefreshInevntory<T>(item.ItemType);
        return res.IsSuccess;
    }

    public async UniTask<bool> CanBuy(T item){
        var balance = ServiceLocator.Instance.GetService<UserBalanceService>();
        if(item.IsFree) return true;
        await UniTask.CompletedTask;
        return balance.GetBalanceInt(item.Currency) >= item.Price;
    }

    public abstract UniTask<bool> CheckFreeItemAvailable();
    public abstract UniTask<bool> ClaimFreeItem();
    public abstract UniTask<int> GetNextFreeItemRefreshTimeInSeconds();
}
}
