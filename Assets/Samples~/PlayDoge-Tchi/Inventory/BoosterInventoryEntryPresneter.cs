using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TA.APIClient.ResponseData;
using System.Linq;
using System;

public class BoosterInventoryEntryPresenter : MonoBehaviour, IDisposable {
    [SerializeField] List<MapEntry> spriteMap = new();
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI quantity;

    public InventoryEntry<Booster> Data {get; private set;}

    public void Create(InventoryEntry<Booster> entry){
        Data = entry;
        quantity.text = entry.quantity.ToString(); 
        GetComponent<Button>().interactable = entry.quantity > 0;
        if(spriteMap.Where(m => m.boosterId == entry.item.ShopId).Any()){
            var sprite = spriteMap.First(m => m.boosterId == entry.item.ShopId).boosterIconSprite;
            icon.sprite = sprite;
        }
    }

    public void Dispose(){
        Destroy(gameObject);
    }
}
