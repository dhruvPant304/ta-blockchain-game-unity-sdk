using System;
using TA.APIClient;
using TA.Services;
using UnityEngine;
using TA.InGameShop;
using System.Collections.Generic;
using System.Linq;

namespace TA.Menus{
public class TAMenuService : Service<TAMenuService>{
    [SerializeField] BuyCreditsMenu _buyCreditsMenu;
    [SerializeField] BuyCreditsMenu inGameBuyCreditMenu;
    [SerializeField] ProfileDetailsPage _profilePage;
    [SerializeField] SettingsPage _settingsPage;
    [SerializeField] GameOverMenu _gameOverMenu;
    [SerializeField] LeaderBoardView _leaderBoardView;

    public event Action OnBuyCreditsMenuOpen;
    public event Action OnBuyCreditsMenuClosed;

    public event Action OnInGameCreditShopOpen;
    public event Action OnInGameCreditShopClosed;

    List<ShopView> _shops = new(); //TODO: Should have an abstraction like View instead of using concrete type

    protected override void OnInitialize(){
        _shops.AddRange(GetComponentsInChildren<ShopView>());
    }

    public void OpenBuyCreditsMenu(){
        CloseAll();
        _buyCreditsMenu.Open();
        OnBuyCreditsMenuOpen?.Invoke();
    }

    public void CloseBuyCreditMenu(){
        var config = ServiceLocator.Instance.GetService<APIConfigProviderService>().APIConfig;

        if(config.inAppCreditPurchaseAvailable){
            _buyCreditsMenu.Close();
            OnBuyCreditsMenuClosed?.Invoke();
        }

        if(config.showShopInBrowser){
            Application.OpenURL(config.shopUrl);
        }
    }

    public void OpenGameOverMenu(){
        _gameOverMenu.ShowEndGameMenu();
    }

    public void OpenInGameCreditShop(){
        CloseAll();
        inGameBuyCreditMenu.Open();
        OnInGameCreditShopOpen?.Invoke();
    }

    public void CloseInGameCreditShop(){
        Debug.Log("Closing in game credit shop");
        inGameBuyCreditMenu.Close();
        OnInGameCreditShopClosed?.Invoke();
    }

    public void CloseCreditsShop(BuyCreditsMenu menu){
        if(menu == _buyCreditsMenu) CloseBuyCreditMenu();
        if(menu == inGameBuyCreditMenu) CloseInGameCreditShop();
    }

    public void OpenProfilePage(){
        CloseAll();
        _profilePage.Show();
    }

    public void CloseProfilePage(){
        _profilePage.Hide();
    }

    public void OpenSettingsPage(){
        CloseAll();
        _settingsPage.Show();
    }

    public void CloseSettingsPage(){
        _settingsPage.Hide();
    }

    public void OpenLeaderBoardPage(){
        CloseAll();
        _leaderBoardView.Show();
    }

    public void CloseLeaderBoardPage(){
        _leaderBoardView.Hide();
    }

    public void OpenShopView<T>() where T : IShopItem {
        var shop = GetShopView<T>();
        if(shop) {
            CloseAll();
            shop.Show();
        }else{
            Debug.LogError($"Shop for product type {typeof(T)}, no found in children");
        }
    }

    private ShopView GetShopView<T>() where T : IShopItem {
        if(_shops.Where(s => s is ShopView<T>).Any()){
            return _shops.First(s => s is ShopView<T>);
        }
        return null;
    }

    public void CloseAll(){
        CloseBuyCreditMenu();
        CloseProfilePage();
        CloseSettingsPage();
        CloseInGameCreditShop();
        CloseLeaderBoardPage();
        _shops.ForEach(s => s.Hide());
    }
}}
