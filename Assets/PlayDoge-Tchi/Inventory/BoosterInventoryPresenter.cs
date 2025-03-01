using System;
using System.Collections.Generic;
using TA.APIClient.ResponseData;
using TA.Services;
using TA.UserInventory;
using UnityEngine;

public class BoosterInventoryPresenter : MonoBehaviour {
    [SerializeField] BoosterInventoryEntryPresenter inventoryDisplayTemplate;
    [SerializeField] Transform contentRoot; 

    List<IDisposable> _disposables = new();
    UserInventoryService _invetory;

    void Start(){
        inventoryDisplayTemplate.gameObject.SetActive(false);
        _invetory = ServiceLocator.Instance.GetService<UserInventoryService>();
        _invetory.OnInventoryUpdate += OnInventoryUpdate;
        Show();
    }

    void Show(){
        var inventoryService = ServiceLocator.Instance.GetService<UserInventoryService>();
        var inventory = inventoryService.GetInventory().OfType(Booster.BOOSTER_ITEM_TYPE).As<Booster>();

        var created = new List<BoosterInventoryEntryPresenter>();
        foreach(var entry in inventory){
            var entryDisplay = Instantiate(inventoryDisplayTemplate, contentRoot);
            entryDisplay.Create(entry);
            created.Add(entryDisplay);
        }
        DisposeAll();
        created.ForEach((ed) => ed.gameObject.SetActive(true));
        _disposables.AddRange(created);
    }

    void OnInventoryUpdate(List<InventoryEntry> entries){
        Show();
    }

    void DisposeAll(){
        _disposables.ForEach((d) => d.Dispose());
        _disposables = new();
    }
}
