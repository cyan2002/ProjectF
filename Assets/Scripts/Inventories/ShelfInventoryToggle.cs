using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfInventoryToggle : MonoBehaviour
{
    private bool PlayerInRange = false;
    private bool InventoryOpen = false;
    public GameObject controlledGrid;

    private void Update()
    {
        if (PlayerInRange && Input.GetKeyDown(KeyCode.M) && !InventoryOpen)
        {
            controlledGrid.SetActive(true);
            InventoryOpen = true;
        }
        else if(!PlayerInRange || Input.GetKeyDown(KeyCode.M) && InventoryOpen)
        {
            controlledGrid.SetActive(false);
            InventoryOpen = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInRange = false;
        controlledGrid.SetActive(false);
    }
}
