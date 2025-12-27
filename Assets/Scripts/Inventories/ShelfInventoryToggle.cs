using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfInventoryToggle : MonoBehaviour
{
    private bool PlayerInRange = false;
    private bool InventoryOpen = false;
    public GameObject controlledGrid;

    private void Start(){
        PlayerInput.HandleM += ToggleShelf;
    }

    public ItemGrid ReturnGrid()
    {
        return controlledGrid.GetComponent<ItemGrid>();
    }

    void ToggleShelf(){
        if (PlayerInRange && !InventoryOpen)
        {
            controlledGrid.SetActive(true);
            InventoryOpen = true;
        }
        else if(PlayerInRange && InventoryOpen){
            controlledGrid.SetActive(false);
            InventoryOpen = false;
        }
        else if(!PlayerInRange && InventoryOpen)
        {
            controlledGrid.SetActive(false);
            InventoryOpen = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            PlayerInRange = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            PlayerInRange = false;
            controlledGrid.SetActive(false);
            InventoryOpen = false;
        }
    }
}
