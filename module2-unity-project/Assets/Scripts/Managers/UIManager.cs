using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public InventoryManager inventoryManager;

    public TextMeshProUGUI pumpkinsText;
    public TextMeshProUGUI lanternsText;
    public TextMeshProUGUI coffinsText;

    public GameObject inventoryPanel;

    public TextMeshProUGUI activeInventoryText;

    public InventoryFull inventoryFull;
    private void Awake()
    {
        inventoryPanel.SetActive(false);
        SetActiveInventory(InventoryItem.Pumpkin);
    }

    public void UpdateInventoryUI()
    {
        pumpkinsText.text = $"Pumpkins: {inventoryManager.inventory[InventoryItem.Pumpkin]}";
        lanternsText.text = $"Lanterns: {inventoryManager.inventory[InventoryItem.Lantern]}";
        coffinsText.text = $"Coffins: {inventoryManager.inventory[InventoryItem.Coffin]}";
    }

    public void ShowInventory(bool show)
    {
        inventoryPanel.SetActive(show);
    }

    public void SetPumpkinActive()
    {
        SetActiveInventory(InventoryItem.Pumpkin);
    }

    public void SetLanternActive()
    {
        SetActiveInventory(InventoryItem.Lantern);
    }

    public void SetCoffinActive()
    {
        SetActiveInventory(InventoryItem.Coffin);
    }

    void SetActiveInventory(InventoryItem item)
    {
        inventoryManager.activeItem = item;
        activeInventoryText.text = $"Active Inventory: {item}";
    }

    public void ShowInventoryFull()
    {
        inventoryFull.ShowInventoryFull();
    }
}
