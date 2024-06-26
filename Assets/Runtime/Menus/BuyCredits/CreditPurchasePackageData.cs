using System;
using System.Collections.Generic;
using UnityEngine;

namespace TA.Menus{

[CreateAssetMenu(menuName="TA/Purchase Pacakge Data", fileName="PurchasePackageData")]
public class CreditPurchasePackageData : ScriptableObject{
    public List<PurchasePackage> packages;
}

[Serializable]
public class PurchasePackage{
    public int creditAmount;
    public int price;
}
}
