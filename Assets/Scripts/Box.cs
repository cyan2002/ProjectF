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

    // Update is called once per frame
    void Update()
    {
        if (close && Input.GetKeyDown(KeyCode.P))
        {
            DumpIntoPlayer();
        }
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
        for(int i = 0; i < items.Count; i++)
        {
            InventoryController.Instance.purchaseItem(items[i]);
        }
        items.Clear();
        sprite.enabled = false;
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
