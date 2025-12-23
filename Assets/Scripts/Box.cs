using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] List<ItemData> items;
    private SpriteRenderer sprite;

    private bool close = false;

    //call to Shop Controller -> ShopController.instance.AddMoney (or whatever the function is)
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    public void AddContents(ItemData ItemToAdd)
    {
        items.Add(ItemToAdd);

        if (items.Count == 0)
        {
            sprite.enabled = false;
        }
        else
        {
            sprite.enabled = true;
        }
    }

    public void DumpIntoPlayer()
    {
        if(close){
            for(int i = items.Count-1; i >= 0; i--)
            {
                //only returns true if the item was added
                if (InventoryController.Instance.purchaseItem(items[i]))
                {
                    items.RemoveAt(i);
                }
            }

            if (items.Count == 0)
            {
                sprite.enabled = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            close = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            close = false;
        }
    }
}
