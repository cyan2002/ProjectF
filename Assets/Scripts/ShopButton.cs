using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    [SerializeField] int cost;
    [SerializeField] ItemData item;
    public void AttemptToBuy()
    {
        if (MoneyManager.Instance.SpendMoney(cost))
        {
            //add to inventory, tell shop inventory to add items to the box
            ShopController.Instance.AddToBox(item);
        }
        else
        {
            //Debug.Log("broke boi");
        }
    }
}
