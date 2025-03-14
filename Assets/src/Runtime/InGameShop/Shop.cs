
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.Services;
using TA.UserInventory;
using TA.UserProfile;
using TA.UserProfile.Balance;

namespace TA.InGameShop{
    public abstract class Shop<T> : IShop<T> where T : class, IShopItem{
        public abstract UniTask<List<T>> GetAllShopItems();

        public bool Buy(T item, int quantity){
            var api = ServiceLocator.Instance.GetService<APIService>();
            var balance = ServiceLocator.Instance.GetService<UserBalanceService>();
            var profile = ServiceLocator.Instance.GetService<UserProfileService>();
            var inventory = ServiceLocator.Instance.GetService<UserInventoryService>();
            var token = profile.LoginToken;
            if(balance.CanSpend(item.Currency, item.Price)){
                balance.SpendCached(item.Currency, item.Price);
                inventory.AddToInventoryCached(item, quantity);
                api.SendBuyItemRequest(item,quantity,token).Forget();
                return true;
            }
            return false;
        }

        public bool CanBuy(T item){
            var balance = ServiceLocator.Instance.GetService<UserBalanceService>();
            if(item.IsFree) return true;
            return balance.CanSpend(item.Currency, item.Price);
        }

        public abstract UniTask<bool> CheckFreeItemAvailable();
        public abstract UniTask<bool> ClaimFreeItem();
        public abstract UniTask<int> GetNextFreeItemRefreshTimeInSeconds();
    }
}
