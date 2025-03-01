using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using TA.UserProfile;
using TA.Services;
using TA.UserInventory;

public class BoosterPurchaseOption : MonoBehaviour , IDisposable {
    [SerializeField] List<MapEntry> spriteMap = new();
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI iconName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI price;
    [SerializeField] CountdownTimer freeBoosterTimer;
    [SerializeField] Button purchaseBtn;

    BoosterShop _shop;
    Booster _booster;

    public Action OnPurchase;
    public Action OnClaimedFree;

    public Booster Item => _booster;

    public async void Create(BoosterShop shop, Booster item){
        _shop = shop;
        _booster = item;

        var profile = ServiceLocator.Instance.GetService<UserProfileService>();

        if(spriteMap.Where(m => m.boosterId == item.ShopId).Any()){
            var sprite = spriteMap.First(m => m.boosterId == item.ShopId).boosterIconSprite;
            icon.sprite = sprite;
        }

        if(description) description.text = item.Description;
        iconName.text = item.DisplayName;

        if(!item.IsFree){
            purchaseBtn.interactable = false;
            var res = shop.CanBuy(item);
            purchaseBtn.interactable = res;
            price.text = "<sprite index=0> " + item.Price.ToString();
            purchaseBtn.onClick.AddListener(Buy);
        }else{
            purchaseBtn.interactable= false;
            await shop.CheckFreeItemAvailable().ContinueWith((res) => purchaseBtn.interactable = res);
            iconName.text += " <sprite index=0>";
            purchaseBtn.onClick.AddListener(Claim);
            shop.GetNextFreeItemRefreshTimeInSeconds().ContinueWith(res => freeBoosterTimer.StartTimer(res)).Forget();
        }
    }

    public void EnsureAvailability(){
        purchaseBtn.interactable = false;
        if(!_booster.IsFree){
            var canBuy = _shop.CanBuy(_booster);
            purchaseBtn.interactable = canBuy;
        }
    }

    void Buy(){
        _shop.Buy(_booster,1);
        OnPurchase?.Invoke();
    }

    async void Claim(){
        purchaseBtn.interactable = false;
        await _shop.ClaimFreeItem();
        UserInventoryService.Get().AddToInventoryCached(_booster,1);
        OnClaimedFree?.Invoke();
    }

    public void Dispose(){
        Destroy(gameObject);
    }
}

[Serializable]
public class MapEntry{
    public int boosterId;
    public Sprite boosterIconSprite;
}
