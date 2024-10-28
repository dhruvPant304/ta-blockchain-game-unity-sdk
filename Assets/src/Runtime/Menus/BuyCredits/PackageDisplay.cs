using TA.IAP;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

namespace TA.Menus{
public abstract class PackageDisplay : MonoBehaviour {
    [SerializeField] Button buyButton;
    PurchasePackage package;
    IAPService _iapService; 
    
    void Start(){
        _iapService = ServiceLocator.Instance.GetService<IAPService>();
    }

    public void AssignPackage(PurchasePackage package){
        this.package = package;
        ShowPackage(package);
    }

    void InitiateIAP(){
        if(package != null){
           _iapService.BuyProductID(package.storeProductId); 
        }
    }

    protected abstract void ShowPackage(PurchasePackage package);
}}
