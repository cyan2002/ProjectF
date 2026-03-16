using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    public List<ItemData> items;

    public ItemData GetByID(string id)
    {
        return items.Find(i => i.itemID == id);
    }
}