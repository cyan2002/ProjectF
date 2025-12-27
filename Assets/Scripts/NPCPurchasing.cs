using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCPurchasing : MonoBehaviour
{
    private List<ItemGrid> Grids = new List<ItemGrid>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            if (collision.gameObject.GetComponent<ShelfInventoryToggle>() != null)
            {
                Grids.Add(collision.gameObject.GetComponent<ShelfInventoryToggle>().ReturnGrid());
                Debug.Log("Adding Grid!");
                for (int i = 0; i < Grids.Count; i++)
                {
                    Debug.Log(Grids[i]);
                }
            }
        }
    }
}
