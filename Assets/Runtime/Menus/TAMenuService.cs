using System;
using TA.Services;
using UnityEngine;

namespace TA.Menus{
public class TAMenuService : Service<TAMenuService>{
    [SerializeField] BuyCreditsMenu _buyCreditsMenu;
    [SerializeField] ProfileDetailsPage _profilePage;

    public event Action OnBuyCreditsMenuOpen;
    public event Action OnBuyCreditsMenuClosed;

    protected override void OnInitialize(){
    }

    public void OpenBuyCreditsMenu(){
        _buyCreditsMenu.Open();
        OnBuyCreditsMenuOpen?.Invoke();
    }

    public void CloseBuyCreditMenu(){
        _buyCreditsMenu.Close();
        OnBuyCreditsMenuClosed?.Invoke();
    }

    public void OpenProfilePage(){
        _profilePage.Show();
    }

    public void CloseProfilePage(){
        _profilePage.Hide();
    }

    public void CloseAll(){
        CloseBuyCreditMenu();
        CloseProfilePage();
    }
}}
