using TMPro;
using UnityEngine;

namespace TA.Menus{
public class BuyPackageButton : MonoBehaviour{
    [SerializeField] TextMeshProUGUI creditAmountText;
    [SerializeField] TextMeshProUGUI priceText;

    public void ShowPackage(PurchasePackage package){
        creditAmountText.text = package.creditAmount.ToString() + " CREDITS";
        priceText.text = "-$ " + package.price.ToString();
    }
}}
