using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{

    public ItemData itemData;
    public bool isTaken = false;

    public int HEIGHT
    {
        get
        {
            if (rotated == false)
            {
                return itemData.height;
            }
            return itemData.width;
        }
    }

    public int WIDTH
    {
        get
        {
            if (rotated == false)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }

    public string getType()
    {
        return itemData.type;
    }

    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;

    internal void Set(ItemData itemData)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        float scale = GetComponentInParent<CanvasScaler>().scaleFactor;

        float tw = ItemGrid.tileSizeWidth * scale;
        float th = ItemGrid.tileSizeHeight * scale;

        // Use WIDTH and HEIGHT so rotation is accounted for
        Vector2 size = new Vector2(
        Mathf.Round(WIDTH * tw),
        Mathf.Round(HEIGHT * th)
        );

        GetComponent<RectTransform>().sizeDelta = size;
    }


    internal void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f);
    }
}
