using System;
using TA.Services;
using UnityEngine;

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

    protected override void OnInitialize(){
    }

    public void OpenBuyCreditsMenu(){
        CloseAll();
        _buyCreditsMenu.Open();
        OnBuyCreditsMenuOpen?.Invoke();
    }

    public void CloseBuyCreditMenu(){
        _buyCreditsMenu.Close();
        OnBuyCreditsMenuClosed?.Invoke();
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

    public void CloseAll(){
        CloseBuyCreditMenu();
        CloseProfilePage();
        CloseSettingsPage();
        CloseInGameCreditShop();
        CloseLeaderBoardPage();
    }
}}
