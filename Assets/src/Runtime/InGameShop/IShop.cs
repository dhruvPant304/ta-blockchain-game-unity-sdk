using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace TA.InGameShop{
public interface IShop<T> where T : IShopItem {
    public UniTask<List<T>> GetAllShopItems();
    public bool CanBuy(T item);
    public bool Buy(T item, int quantity);
    public UniTask<bool> CheckFreeItemAvailable();
    public UniTask<bool> ClaimFreeItem();
    public abstract UniTask<int> GetNextFreeItemRefreshTimeInSeconds();
}
}
