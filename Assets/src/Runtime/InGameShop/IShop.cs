using System.Collections.Generic;

namespace TA.InGameShop{
public interface IShop<T> where T : IShopItem {
    public List<T> AvailableItems {get;}
}
}
