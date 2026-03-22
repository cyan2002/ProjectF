using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class SavedItem
{
    public string itemDataName;
    public int posX;
    public int posY;
    public bool rotated;
    public string itemID;
    public string GridID;
}

[System.Serializable]
public class WorldSaveData
{
    public string sceneName;
    public float playerPosX;
    public float playerPosY;
    public int money;
    public float timeOfDay;
    public int day;
}

[System.Serializable]
public class InventorySaveData
{
    public List<SavedItem> items = new List<SavedItem>();
}

[System.Serializable]
public class FullSaveData
{
    public WorldSaveData world = new WorldSaveData();
    public InventorySaveData inventory = new InventorySaveData();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    [SerializeField] ItemDatabase itemDatabase;
    [SerializeField] GameObject itemPrefab;

    public bool isLoadingFromSave = false;

    // In-memory grid cache for cross-scene travel within a session
    private InventorySaveData sessionGridData = new InventorySaveData();

    private string SavePath => Application.persistentDataPath + "/save.json";

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

    // -------------------------
    // Scene Load
    // -------------------------

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    Debug.Log($"OnSceneLoaded: {scene.name}, isLoadingFromSave: {isLoadingFromSave}");
    
    if (scene.name == "Master") return;

    if (isLoadingFromSave)
    {
        LoadAllGrids();
        WorldSaveData world = LoadWorldState();
        Debug.Log($"Saved scene: {world.sceneName}, loaded scene: {scene.name}, money: {world.money}, time: {world.timeOfDay}");
        StartCoroutine(RestoreWorldState(world, scene.name));
    }
    else
    {
        LoadGridsFromSession();
    }
}

IEnumerator RestoreWorldState(WorldSaveData world, string loadedSceneName)
{
    yield return null;

    Debug.Log($"RestoreWorldState running. MoneyManager null: {MoneyManager.Instance == null}, Clock null: {Clock.Instance == null}");

    if (world.sceneName == loadedSceneName)
    {
        Debug.Log($"Scene match — restoring money: {world.money}, time: {world.timeOfDay}");
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 savedPos = new Vector2(world.playerPosX, world.playerPosY);
            if (rb != null) rb.position = savedPos;
            else player.transform.position = savedPos;
        }

        if (MoneyManager.Instance != null)
            MoneyManager.Instance.SetMoney(world.money);
        else
            Debug.LogWarning("MoneyManager.Instance is null");

        if (Clock.Instance != null)
            Clock.Instance.SetClock(world.timeOfDay, world.day);
        else
            Debug.LogWarning("Clock.Instance is null");
    }
    else
    {
        Debug.LogWarning($"Scene mismatch — world.sceneName: {world.sceneName}, loadedSceneName: {loadedSceneName}");
    }

    isLoadingFromSave = false;
}

    // -------------------------
    // World State
    // -------------------------

    public void SaveWorldState(Vector2 playerPos, int money, float timeOfDay, int day)
    {
        FullSaveData fullData = LoadFullFromDisk();

        // Find the actual game scene, not Master
        string sceneName = "";
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name != "Master")
            {
                sceneName = s.name;
                break;
            }
        }

        fullData.world.sceneName = sceneName;
        fullData.world.playerPosX = playerPos.x;
        fullData.world.playerPosY = playerPos.y;
        fullData.world.money = money;
        fullData.world.timeOfDay = timeOfDay;
        fullData.world.day = day;

        SaveToDisk(fullData);
    }

    public WorldSaveData LoadWorldState()
    {
        return LoadFullFromDisk().world;
    }

    // -------------------------
    // Session Grid Save / Load (memory only, not disk)
    // -------------------------

    public void SaveGridsToSession()
    {
        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        HashSet<string> currentGridIDs = new HashSet<string>();
        foreach (ItemGrid grid in grids)
            currentGridIDs.Add(grid.GridID);

        sessionGridData.items.RemoveAll(i => currentGridIDs.Contains(i.GridID));

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

                    sessionGridData.items.Add(new SavedItem
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
    }

    public void LoadGridsFromSession()
    {
        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Dictionary<string, ItemGrid> gridLookup = new Dictionary<string, ItemGrid>();
        foreach (ItemGrid grid in grids)
        {
            grid.ClearGrid();
            gridLookup[grid.GridID] = grid;
        }

        foreach (SavedItem savedItem in sessionGridData.items)
        {
            if (!gridLookup.ContainsKey(savedItem.GridID)) continue;

            ItemGrid targetGrid = gridLookup[savedItem.GridID];
            ItemData data = itemDatabase.GetByID(savedItem.itemID);
            if (data == null)
            {
                Debug.LogWarning($"ItemData not found for ID: {savedItem.itemID}");
                continue;
            }

            InventoryItem newItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            newItem.rotated = savedItem.rotated;
            newItem.Set(data);

            if (savedItem.rotated)
                newItem.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90f);

            targetGrid.PlaceItem(newItem, savedItem.posX, savedItem.posY);
        }
    }

    // -------------------------
    // Disk Grid Save / Load (explicit save button only)
    // -------------------------

    public void SaveAllGrids()
    {
        FullSaveData fullData = LoadFullFromDisk();

        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        HashSet<string> currentGridIDs = new HashSet<string>();
        foreach (ItemGrid grid in grids)
            currentGridIDs.Add(grid.GridID);

        fullData.inventory.items.RemoveAll(i => currentGridIDs.Contains(i.GridID));

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

                    fullData.inventory.items.Add(new SavedItem
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

        SaveToDisk(fullData);
    }

    public void LoadAllGrids()
    {
        if (!File.Exists(SavePath)) return;

        FullSaveData fullData = LoadFullFromDisk();

        ItemGrid[] grids = FindObjectsByType<ItemGrid>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Dictionary<string, ItemGrid> gridLookup = new Dictionary<string, ItemGrid>();
        foreach (ItemGrid grid in grids)
        {
            grid.ClearGrid();
            gridLookup[grid.GridID] = grid;
        }

        foreach (SavedItem savedItem in fullData.inventory.items)
        {
            if (!gridLookup.ContainsKey(savedItem.GridID)) continue;

            ItemGrid targetGrid = gridLookup[savedItem.GridID];
            ItemData data = itemDatabase.GetByID(savedItem.itemID);
            if (data == null)
            {
                Debug.LogWarning($"ItemData not found for ID: {savedItem.itemID}");
                continue;
            }

            InventoryItem newItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            newItem.rotated = savedItem.rotated;
            newItem.Set(data);

            if (savedItem.rotated)
                newItem.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 90f);

            targetGrid.PlaceItem(newItem, savedItem.posX, savedItem.posY);
        }
    }

    // -------------------------
    // Disk I/O
    // -------------------------

    void SaveToDisk(FullSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    FullSaveData LoadFullFromDisk()
    {
        if (!File.Exists(SavePath))
            return new FullSaveData();

        string json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<FullSaveData>(json);
    }

    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }
}