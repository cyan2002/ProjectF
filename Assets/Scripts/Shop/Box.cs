using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] List<ItemData> items;
    [SerializeField] List<Sprite> boxImages;
    private SpriteRenderer sprite;

    public BoxCollider2D hitbox;

    private bool close = false;

    //call to Shop Controller -> ShopController.instance.AddMoney (or whatever the function is)
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        hitbox.enabled = false;
    }

    public void AddContents(ItemData ItemToAdd)
    {
        items.Add(ItemToAdd);

        if (items.Count == 0)
        {
            hitbox.enabled = false;
            sprite.enabled = false;
        }
        else if (items.Count < 5)
        {
            hitbox.enabled = true;
            sprite.sprite = boxImages[0];
            sprite.enabled = true;
        }
        else if (items.Count < 10)
        {
            hitbox.enabled = true;
            sprite.sprite = boxImages[1];
            sprite.enabled = true;
        }
        else if (items.Count < 15)
        {
            hitbox.enabled = true;
            sprite.sprite = boxImages[2];
            sprite.enabled = true;
        }
        else
        {
            hitbox.enabled = true;
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
                hitbox.enabled = false;
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
