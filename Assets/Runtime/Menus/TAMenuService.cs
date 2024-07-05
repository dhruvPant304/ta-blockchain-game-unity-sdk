using System;
using TA.Services;
using UnityEngine;

namespace TA.Menus{
public class TAMenuService : Service<TAMenuService>{
    [SerializeField] BuyCreditsMenu _buyCreditsMenu;
    [SerializeField] ProfileDetailsPage _profilePage;
    [SerializeField] SettingsPage _settingsPage;

    public event Action OnBuyCreditsMenuOpen;
    public event Action OnBuyCreditsMenuClosed;

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

    public void CloseAll(){
        CloseBuyCreditMenu();
        CloseProfilePage();
        CloseSettingsPage();
    }
}}
