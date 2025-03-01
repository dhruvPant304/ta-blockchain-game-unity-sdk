using TA.Services;
using TA.UserInventory;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoosterInventoryEntryPresenter),typeof(Button))]
public class ConsumeShopItem : MonoBehaviour{
    void Start(){
        GetComponent<Button>().onClick.AddListener(Consume);
    }

    void Consume(){
        var entry = GetComponent<BoosterInventoryEntryPresenter>();
        var inventory = ServiceLocator.Instance.GetService<UserInventoryService>();
        inventory.TakeFromInventoryCached(entry.Data.item);
    }
}
