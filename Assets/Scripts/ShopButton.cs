using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopButton : MonoBehaviour
{
    [SerializeField] int cost;
    [SerializeField] ItemData item;

    //when buying things that need to happen:
    //check if possible to buy, if possible then:
    //play sounds of transaction
    //add to shipping container
    //update money gui
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
