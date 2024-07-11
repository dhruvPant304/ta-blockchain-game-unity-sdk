using TMPro;
using UnityEngine;

namespace TA.Menus{
public class InGamePackageOption : PackageDisplay {
    [SerializeField] TextMeshProUGUI credit;
    [SerializeField] TextMeshProUGUI price;

    public override void ShowPackage(PurchasePackage package){
        credit.text = package.creditAmount.ToString();
        price.text = "$" + package.price.ToString();
    }
}}
