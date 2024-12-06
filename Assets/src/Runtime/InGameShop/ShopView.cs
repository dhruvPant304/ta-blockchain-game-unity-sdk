using UnityEngine;

namespace TA.InGameShop{
public abstract class ShopView<TItem> : MonoBehaviour where TItem : IShopItem {
    public abstract void ShowShopController<TShop>(TShop shop) where TShop: IShop<TItem>;
}
}
