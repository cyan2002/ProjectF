using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    ShelfInventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField]
    private CanvasScaler canvasGroup;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public Camera uiCamera;

    [SerializeField] int gridSizeWidth = 10;
    [SerializeField] int gridSizeHeight = 9;

    public ShelfInventoryItem PickUpItem(int x, int y)
    {
        print(inventoryItemSlot[x, y].gameObject.name);
        ShelfInventoryItem toReturn = inventoryItemSlot[x, y];
        inventoryItemSlot[x, y] = null;
        return toReturn;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height)
    {
        inventoryItemSlot = new ShelfInventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    //returns the Grid position of the tile where the mouse currently is
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            mousePosition,
            null, // for Screen Space - Overlay
            out localPoint
        );

        // Convert pivot-based coords → top-left coords
        Vector2 rectSize = rectTransform.rect.size;
        Vector2 topLeftOrigin = new Vector2(
            -rectSize.x * rectTransform.pivot.x,
             rectSize.y * (1 - rectTransform.pivot.y)
        );

        // Top-left–relative coordinate
        Vector2 TL = localPoint - topLeftOrigin;

        int x = Mathf.FloorToInt(TL.x / tileSizeWidth);
        int y = Mathf.FloorToInt(-TL.y / tileSizeHeight);  // NOTICE THE '-' HERE

        return new Vector2Int(x, y);
    }

    public void PlaceItem(ShelfInventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform itemRT = inventoryItem.GetComponent<RectTransform>();
        itemRT.SetParent(rectTransform, false);

        // Position item inside the grid
        Vector2 rectSize = rectTransform.rect.size;
        Vector2 pivot = rectTransform.pivot;

        // Convert pivot origin → top-left origin
        Vector2 topLeftOrigin = new Vector2(
            -rectSize.x * pivot.x,
             rectSize.y * (1 - pivot.y)
        );

        // Item position in top-left coordinate space
        float x = topLeftOrigin.x + posX * tileSizeWidth + tileSizeWidth * 0.5f * inventoryItem.itemData.width;
        float y = topLeftOrigin.y - posY * tileSizeHeight - tileSizeHeight * 0.5f * inventoryItem.itemData.height;

        //adding it into the inventory array to keep track of it
        inventoryItemSlot[posX, posY] = inventoryItem;

        itemRT.localPosition = new Vector2(x, y);
    }
}
