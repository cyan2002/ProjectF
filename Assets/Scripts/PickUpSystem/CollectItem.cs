using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    [SerializeField]
    private InventorySO inventoryData;

    [field: SerializeField]
    public ItemSO InventoryItem { get; private set; }

    private ItemSO copy_inventoryItem;

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            int reminder = inventoryData.AddItem(item.InventoryItem, item.Quantity);
            if (reminder == 0)
                item.DestroyItem();
            else
                item.Quantity = reminder;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            addItem();
        }
    }

    private void addItem()
    {
        copy_inventoryItem = InventoryItem;
        int reminder = inventoryData.AddItem(copy_inventoryItem, 1);
        //if collected all, destroy the rest of it
        //if (reminder == 0)
            //Destroy(copy_inventoryItem);
        //if did not collect it all, leave the remainder
        //else
            //InventoryItem.
    }
}