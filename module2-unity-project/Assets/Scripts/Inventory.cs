using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour, IHittable
{
    public InventoryItem item;

    [HideInInspector]
    public UnityEvent<Inventory> OnItemCollected;

    public void Awake()
    {
        if (OnItemCollected == null) OnItemCollected = new UnityEvent<Inventory>();
    }

    public void Hit(GameObject otherGameObject)
    {
        OnItemCollected.Invoke(this);
    }

}
