using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TA.APIClient;
using TA.InGameShop;
using TA.Services;
using TA.UserInventory;
using TA.UserProfile;
using UnityEngine;
using UnityEngine.UI;

public class BoosterShopPresenter: ShopView<Booster> {
    [SerializeField] BoosterPurchaseOption paidBoosterViewPrefab;
    [SerializeField] BoosterPurchaseOption freeBoosterViewPrefab;

    [SerializeField] Transform paidBoosterRoot;
    [SerializeField] Transform freeBoosterRoot;

    [SerializeField] Button closeButton;

    BoosterShop _boosterShop;
    List<BoosterPurchaseOption> _options = new();

    bool isOpen;

    void Start(){
        isOpen = gameObject.activeSelf;
        Hide();
        var locator = ServiceLocator.Instance; 
        _boosterShop = new BoosterShop(locator.GetService<APIService>(), 
                locator.GetService<UserProfileService>(),
                locator.GetService<UserInventoryService>());

        closeButton.onClick.AddListener(Hide);
    }

    async UniTask FetchShop(){
        var items = await _boosterShop.GetAllShopItems();

        foreach(var item in items){
            if(item.IsFree) CreateFreeBooosterOption(_boosterShop, item);
            else CreatePaidBoosterOption(_boosterShop, item);
        }
    }

    async UniTask<Booster> GetFreeBooster(){
        var items = await _boosterShop.GetAllShopItems();

        foreach(var item in items){
            if(item.IsFree) return item;
        }

        return null;
    }

    public override async void Show(){
        if(isOpen) return;
        isOpen = true;
        await FetchShop();
        gameObject.SetActive(true);
    }

    public override void Hide(){
        if(!isOpen) return;
        isOpen =false;

        gameObject.SetActive(false);
        _options.ForEach(d => d.Dispose());
        _options = new();
    }

    void CreatePaidBoosterOption(BoosterShop shop, Booster item){
        var option = Instantiate(paidBoosterViewPrefab, paidBoosterRoot);
        option.Create(shop, item);
        option.gameObject.SetActive(true);
        option.OnPurchase += OnPurchase;
        _options.Add(option);
    }

    void CreateFreeBooosterOption(BoosterShop shop, Booster item){
        var option = Instantiate(freeBoosterViewPrefab, freeBoosterRoot);
        option.Create(shop, item);
        option.gameObject.SetActive(true);
        option.OnClaimedFree += OnClaimedFree;
        _options.Add(option);
    }

    void OnPurchase(){
        _options.ForEach((opt) => opt.EnsureAvailability());
    }

    async void OnClaimedFree(){
        var freeBooster = await GetFreeBooster();
        var lastFreeOption = _options.FirstOrDefault((opt) => opt.Item .isFree);
        if(freeBooster != null){
            CreateFreeBooosterOption(_boosterShop, freeBooster);
        }

        _options.Remove(lastFreeOption);
        if(lastFreeOption != null) lastFreeOption.Dispose();
    }
}
