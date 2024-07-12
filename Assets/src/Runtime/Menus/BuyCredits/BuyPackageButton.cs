using TMPro;
using UnityEngine;

namespace TA.Menus{
public class BuyPackageButton : PackageDisplay{
    [SerializeField] TextMeshProUGUI creditAmountText;
    [SerializeField] TextMeshProUGUI priceText;

    public override void ShowPackage(PurchasePackage package){
        creditAmountText.text = package.creditAmount.ToString() + " CREDITS";
        priceText.text = "-$ " + package.price.ToString();
    }
}}
