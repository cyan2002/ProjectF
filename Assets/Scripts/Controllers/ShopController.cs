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

        //Event from PlayerInput
        PlayerInput.HandleB += ShopToggle;
        PlayerInput.HandleE += PlaceInInventory;
    }

    void ShopToggle(){
        //open inventory
        if (!ShopActive)
        {
            ShopCanvas.gameObject.SetActive(true);
            ShopActive = true;
        }
        else if(ShopActive)
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

    private void PlaceInInventory(){
        ShippingBox.DumpIntoPlayer();
    }

    public void AddToBox(ItemData ItemsToAdd)
    {
        ShippingBox.AddContents(ItemsToAdd);
    }
}
