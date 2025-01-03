public interface IShopItem {
    public int ShopId {get;}
    public string DisplayName {get;}
    public string Description {get;}
    public string Currency {get;}
    public int Price {get;}
    public static string ItemType {get;}
    public bool IsFree {get;}
}
