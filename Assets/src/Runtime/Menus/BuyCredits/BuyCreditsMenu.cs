using System.Collections.Generic;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public class BuyCreditsMenu : MonoBehaviour{
    [SerializeField] GameObject buyCreditPanel;
    [SerializeField] Button closeButton;
    [SerializeField] CreditPurchasePackageData creditPurchasePackageData; 
    [SerializeField] PackageDisplay buttonPrefab;
    [SerializeField] Transform buttonHolder;

    void Start(){
        closeButton.onClick.AddListener(CloseUsingService);
        Close();
    }

    public void Open(){
        buyCreditPanel.SetActive(true);
        ShowPackages();
    }

    public void Close(){
        buyCreditPanel.SetActive(false);
        Debug.Log("Buy Credit panel closed");
    }

    void CloseUsingService(){
        ServiceLocator.Instance.GetService<TAMenuService>().CloseCreditsShop(this);
    }

    List<PackageDisplay> _buttons = new();

    PackageDisplay CreateButton(PurchasePackage package){
        var button = Instantiate(buttonPrefab, buttonHolder);
        button.AssignPackage(package);
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
