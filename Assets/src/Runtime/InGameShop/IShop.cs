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
}
