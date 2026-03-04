using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Managers")]
    public InventoryManager inventoryManager;
    public UIManager uiManager;
    public GameObject inventory;

    [Header("Character Controller")]
    public Character character;

    [Header("Game system objects")]
    public Inventory lantern;
    public GameObject barriers1;
    public Toggle toggle1;
    public WallEye wallEye;
    public Door door;

    [Header("Prefabs")]
    public Inventory pumpkinPrefab;
    public Inventory lanternPrefab;
    public Inventory coffinPrefab;

    // the level manager is responsible for connecting the core game system events
    // notice that these events have arguments - it's not possible to pass arguments to
    // events in Unity when using the Editor (what we did in Module 1)
    // arguments make the events more flexible

    private void Start()
    {
        // inventory events
        inventoryManager.OnInventoryChanged.AddListener(uiManager.UpdateInventoryUI);
        inventoryManager.OnInventoryChanged.AddListener(LockDoorInventory);
        inventoryManager.OnInventorySpawned.AddListener(SpawnInventory);
        inventoryManager.OnInventoryFull.AddListener(uiManager.ShowInventoryFull);

        // this unlocks the door for this SPECIFIC lantern
        //lantern.OnItemCollected.AddListener(LockDoorItemPickup);

        foreach (Transform child in inventory.transform)
        {
            Inventory inventory = child.GetComponent<Inventory>();
            inventory.OnItemCollected.AddListener(inventoryManager.PickUpInventory);
        }
        
        // game system events
        foreach (Transform child in barriers1.transform)
        {
            Barrier barrier = child.GetComponent<Barrier>();
            toggle1.OnToggle.AddListener(barrier.Move);
        }

        toggle1.OnToggle.AddListener(wallEye.OpenClose);
        wallEye.OnEyeStateChanged.AddListener(LockDoorWallEye);

        character.OnInventoryShown.AddListener(uiManager.ShowInventory);
        character.OnItemDropped.AddListener(inventoryManager.DropInventory);
    }

    void LockDoorInventory()
    {
        if (inventoryManager.inventory[InventoryItem.Lantern] > 0)
        {
            door.SetLock(false);
        }
        else
        {
            door.SetLock(true);
        }
    }
    
    void LockDoorWallEye(WallEyeState eyeState)
    {
        if (eyeState == WallEyeState.Defeated)
        {
            door.SetLock(false);
        }
        else
        {
            door.SetLock(true);
        }
    }

    public void SpawnInventory(InventoryItem item)
    {
        switch (item)
        {
            case InventoryItem.Pumpkin:
                SpawnInventoryPrefab(pumpkinPrefab);
                break;
            case InventoryItem.Lantern:
                SpawnInventoryPrefab(lanternPrefab);
                break;
            case InventoryItem.Coffin:
                SpawnInventoryPrefab(coffinPrefab);
                break;
        }
    }

    void SpawnInventoryPrefab(Inventory prefab)
    {
        Inventory inventory = Instantiate(prefab);
        inventory.OnItemCollected.AddListener(inventoryManager.PickUpInventory);

        Vector3 forwardOffset = character.transform.forward * 1.0f;
        inventory.transform.position = character.transform.position + forwardOffset;
    }
}
