using UnityEngine;

namespace TA.InGameShop{
public abstract class ShopView<TItem> : ShopView where TItem : IShopItem {
}

public abstract class ShopView : MonoBehaviour{
    public abstract void Show();
    public abstract void Hide();
}
}
