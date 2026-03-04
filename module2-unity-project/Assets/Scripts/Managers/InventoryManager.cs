using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryManager : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnInventoryChanged;

    [HideInInspector]
    public UnityEvent<InventoryItem> OnInventorySpawned;

    [HideInInspector]
    public UnityEvent OnInventoryFull;

    public Dictionary<InventoryItem, int> inventory = new Dictionary<InventoryItem, int>();

    public InventoryItem activeItem = InventoryItem.Pumpkin;
    public void Awake()
    {
        if (OnInventoryChanged == null)
            OnInventoryChanged = new UnityEvent();

        if (OnInventorySpawned == null)
            OnInventorySpawned = new UnityEvent<InventoryItem>();

        if (OnInventoryFull == null)
            OnInventoryFull = new UnityEvent();

        inventory[InventoryItem.Pumpkin] = 0;
        inventory[InventoryItem.Lantern] = 0;
        inventory[InventoryItem.Coffin] = 0;
    }

    public void PickUpInventory(Inventory inventoryObject)
    {
        if (inventory[inventoryObject.item] < 2)
        {
            inventory[inventoryObject.item] += 1;
            OnInventoryChanged.Invoke();

            Destroy(inventoryObject.gameObject);
        }
        else
        {
            OnInventoryFull.Invoke();
        }
    }

    public void DropInventory()
    {
        // we now just drop the active item, we don't need any additional parameters
        if (inventory[activeItem] > 0)
        {
            inventory[activeItem] -= 1;
            OnInventoryChanged.Invoke();
            OnInventorySpawned.Invoke(activeItem);
        }
    }

}
