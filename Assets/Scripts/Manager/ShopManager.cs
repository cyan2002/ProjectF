using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public Canvas shopInventoryCanvas;
    public RectTransform shopMenu;
    private bool ShopActive = false;

    void Awake()
    {
        Instance = this;
        shopMenu.gameObject.SetActive(false);

        //inventory controller wants the whole canvas, not just the shop UI
        InventoryController.Instance.canvasTransform = shopInventoryCanvas.GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        PlayerInput.HandleB += ShopToggle;
    }

    void OnDisable()
    {
        PlayerInput.HandleB -= ShopToggle;
    }

    void ShopToggle()
    {
        //open inventory
        if (!ShopActive)
        {
            shopMenu.gameObject.SetActive(true);
            ShopActive = true;
        }
        else if (ShopActive)
        {
            shopMenu.gameObject.SetActive(false);
            ShopActive = false;
        }
    }
}
