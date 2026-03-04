using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFull : MonoBehaviour
{
    Animator animator;
    public void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void ShowInventoryFull()
    {
        gameObject.SetActive(true);
        animator.SetTrigger("SetShake");
    }

    public void HideInventoryFull()
    {
        gameObject.SetActive(false);
    }
}
