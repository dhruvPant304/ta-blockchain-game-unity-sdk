using TA.Menus;
using TA.Services;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenBoosterShop : MonoBehaviour{
    void Start(){
        var menu = ServiceLocator.Instance.GetService<TAMenuService>();
        GetComponent<Button>().onClick.AddListener(menu.OpenShopView<Booster>);
    }
}
