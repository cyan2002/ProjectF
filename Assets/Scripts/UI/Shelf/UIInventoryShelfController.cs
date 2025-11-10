using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryShelfController : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPageShelf inventoryUI;

    

    private void Start()
    {
        inventoryUI.InitializeInventoryUI(90);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
            }
            else
            {
                inventoryUI.Hide();
            }

        }
    }
}
