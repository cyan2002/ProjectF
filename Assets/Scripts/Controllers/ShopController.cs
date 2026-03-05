using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController Instance { get; private set; }
    public InventoryItem itemPrefab;

    public Box ShippingBox; //find in scene

    private void Awake()
    {
        Instance = this;

        //Event from PlayerInput
        PlayerInput.HandleE += PlaceInInventory;

        Box[] all = FindObjectsByType<Box>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Box found = System.Array.Find(all, t => t.name == "Box");

        if (found != null)
        {
            ShippingBox = found;
        }
        else
        {
            //will play in the first scene because there is no box
            //Debug.Log("couldn't find box!");
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
        if(ShippingBox != null)
        {
            ShippingBox.DumpIntoPlayer();
        }
        return;
    }

    public void AddToBox(ItemData ItemsToAdd)
    {
        ShippingBox.AddContents(ItemsToAdd);
    }
}
