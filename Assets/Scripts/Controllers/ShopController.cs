using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }
    public InventoryItem itemPrefab;

    public GameObject ShopCanvas;

    public Box ShippingBox;

    private bool ShopActive = false;

    private void Awake()
    {
        Instance = this;
        ShopCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        //open inventory
        if (Input.GetKeyDown(KeyCode.B) && !ShopActive)
        {
            ShopCanvas.gameObject.SetActive(true);
            ShopActive = true;
        }
        else if(Input.GetKeyDown(KeyCode.B) && ShopActive)
        {
            ShopCanvas.gameObject.SetActive(false);
            ShopActive = false;
        }
    }

    //private void testAdd()
    //{
        //addItemsToInventory();
    //}

    //private void addItemsToInventory()
    //{
        //InventoryController.Instance.purchaseItem(items[0]);
    //}

    public void AddToBox(ItemData ItemsToAdd)
    {
        ShippingBox.AddContents(ItemsToAdd);
    }
}
