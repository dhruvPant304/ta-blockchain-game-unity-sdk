using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.Services;
using TA.UserProfile;

namespace TA.InGameShop{
public interface IShop<T> where T : IShopItem {
    public List<T> AvailableItems {get;}
    public List<T> PaidItems => AvailableItems.Where( i => !i.IsFree).ToList();
    public List<T> FreeItems => AvailableItems.Where( i => i.IsFree).ToList();
}

public abstract class Shop<T> : IShop<T> where T : IShopItem{
    public List<T> AvailableItems => GetAllShopItems(); 
    public abstract List<T> GetAllShopItems();
    public async UniTask<bool> Buy(T item, int quantity){
        var api = ServiceLocator.Instance.GetService<APIService>();
        var profile = ServiceLocator.Instance.GetService<UserProfileService>();
        var token = profile.LoginToken;
        var res = await api.SendBuyBoosterRequest(item,quantity,token);
        await profile.UpdateUserBalance();
        return res.IsSuccess;
    }
}
}
