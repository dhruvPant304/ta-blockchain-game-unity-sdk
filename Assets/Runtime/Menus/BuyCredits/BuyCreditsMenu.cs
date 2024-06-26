using System.Collections.Generic;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class BuyCreditsMenu : MonoBehaviour{
    [SerializeField] GameObject buyCreditPanel;
    [SerializeField] Button closeButton;
    [SerializeField] CreditPurchasePackageData creditPurchasePackageData; 
    [SerializeField] BuyPackageButton buttonPrefab;
    [SerializeField] Transform buttonHolder;

    void Awake(){
        Close();
        closeButton.onClick.AddListener(CloseUsingService);
    }

    public void Open(){
        buyCreditPanel.SetActive(true);
        ShowPackages();
    }

    public void Close(){
        buyCreditPanel.SetActive(false);
    }

    void CloseUsingService(){
        ServiceLocator.Instance.GetService<TAMenuService>().CloseBuyCreditMenu();
    }

    List<BuyPackageButton> _buttons = new();

    BuyPackageButton CreateButton(PurchasePackage package){
        var button = Instantiate(buttonPrefab, buttonHolder);
        button.ShowPackage(package);
        _buttons.Add(button);
        return button;
    }

    void ClearAll(){
        foreach(var button in _buttons){
            Destroy(button.gameObject);
        }

        _buttons.Clear();
    }

    void ShowPackages(){
        ClearAll();
        foreach(var package in creditPurchasePackageData.packages){
            CreateButton(package);
        }
    }
}}
