using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfInventoryController : MonoBehaviour
{
    [HideInInspector]
    public ItemGrid selectedItemGrid;

    ShelfInventoryItem selectedItem;

    private void Update()
    {
        if(selectedItemGrid == null) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int tileGridPosition = selectedItemGrid.GetTileGridPosition(Input.mousePosition);

            if(selectedItem == null)
            {
                Debug.Log("1");
                selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
            }
            else
            {
                Debug.Log("2");
                selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y);
            }
        }
    }
}
