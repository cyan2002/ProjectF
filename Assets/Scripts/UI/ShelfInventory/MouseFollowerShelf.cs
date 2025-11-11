using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowerShelf : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private UIInventoryItemShelf item;

    public void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        item = GetComponentInChildren<UIInventoryItemShelf>();
    }

    public void SetData(Sprite sprite, int quantity)
    {
        item.SetData(sprite, quantity);
    }
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void Toggle(bool val)
    {
        //Debug.Log($"Item toggled {val}");
        gameObject.SetActive(val);
    }
}