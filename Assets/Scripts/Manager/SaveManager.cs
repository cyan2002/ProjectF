using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SavedItem
{
    public string itemDataName; // matches the ScriptableObject name
    public int posX;
    public int posY;
    public bool rotated;
    public string itemID;
    public string GridID; // "Player", "Tank", "Shelf", etc.
}


//contains the actual data for each saved item (Position, grid type, etc)
[System.Serializable]
public class InventorySaveData
{
    public List<SavedItem> items = new List<SavedItem>();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] ItemDatabase itemDatabase;
    [SerializeField] GameObject itemPrefab;

    private string SavePath => Application.persistentDataPath + "/inventory.json";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Called after a new scene loads — restore items into its grids
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("loading!");
        LoadAllGrids();
    }

    public void SaveAllGrids()
    {
        // Load existing save data so we don't wipe other scenes' data
        InventorySaveData saveData = LoadFromDisk();

        // Get all grids currently in the scene
        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        // Remove any saved entries that belong to grids in THIS scene
        // (we're about to re-save them fresh)
        HashSet<string> currentGridIDs = new HashSet<string>();
        foreach (ItemGrid grid in grids)
            currentGridIDs.Add(grid.GridID);

        saveData.items.RemoveAll(i => currentGridIDs.Contains(i.GridID));

        // Now re-add current state of each grid
        foreach (ItemGrid grid in grids)
        {
            List<InventoryItem> seen = new List<InventoryItem>();

            for (int x = 0; x < grid.gridSizeWidth; x++)
            {
                for (int y = 0; y < grid.gridSizeHeight; y++)
                {
                    InventoryItem item = grid.inventoryItemSlot[x, y];
                    if (item == null || seen.Contains(item)) continue;
                    seen.Add(item);

                    saveData.items.Add(new SavedItem
                    {
                        itemID = item.itemData.itemID,
                        GridID = grid.GridID,
                        posX = item.onGridPositionX,
                        posY = item.onGridPositionY,
                        rotated = item.rotated
                    });
                }
            }
        }

        SaveToDisk(saveData);
    }

    public void LoadAllGrids()
    {
        if (!File.Exists(SavePath)) return;

        InventorySaveData saveData = LoadFromDisk();

        // Build a lookup of gridID → ItemGrid for grids in this scene
        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Dictionary<string, ItemGrid> gridLookup = new Dictionary<string, ItemGrid>();
        foreach (ItemGrid grid in grids)
        {
            grid.ClearGrid(); // ← wipe first
            gridLookup[grid.GridID] = grid;
        }

        foreach (SavedItem savedItem in saveData.items)
        {
            if (!gridLookup.ContainsKey(savedItem.GridID)) continue;

            ItemGrid targetGrid = gridLookup[savedItem.GridID];
            ItemData data = itemDatabase.GetByID(savedItem.itemID);
            if (data == null)
            {
                Debug.LogWarning($"ItemData not found for ID: {savedItem.itemID}");
                continue;
            }

            // Instantiate and set up the item
            InventoryItem newItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            newItem.rotated = savedItem.rotated;
            newItem.Set(data);

            // Apply rotation visually if needed
            if (savedItem.rotated)
                newItem.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90f);

            // Place directly — no overlap check needed since we saved valid positions
            targetGrid.PlaceItem(newItem, savedItem.posX, savedItem.posY);
        }
    }

    void SaveToDisk(InventorySaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    InventorySaveData LoadFromDisk()
    {
        if (!File.Exists(SavePath))
            return new InventorySaveData();

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<InventorySaveData>(json);
    }
}