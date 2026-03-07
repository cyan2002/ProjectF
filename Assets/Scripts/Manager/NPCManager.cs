using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//handles spawning NPCs and line management for check out
public class NPCManager : MonoBehaviour
{
    public List<GameObject> NPCs = new List<GameObject>();
    public Transform spawnPoint;
    public Transform parentObject;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    [SerializeField]
    private bool currentOpen = false;
    private List<NPC_Controller> activeNPCs = new List<NPC_Controller>();

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private void OnEnable()
    {
        Clock.OnHourChanged += HandleTimeChanged;
    }

    private void OnDisable()
    {
        Clock.OnHourChanged -= HandleTimeChanged;
    }

    private void HandleTimeChanged(int hour)
    {
        switch (hour)
        {
            case 9:
                currentOpen = true; //open shop
                StartCoroutine(SpawnLoop());
                break;
            case 17:
                currentOpen = false; //close shop
                CloseShop();
                //make all NPCs enter leaving mode
                break;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (currentOpen) // keeps running forever
        {
            // Wait a random interval
            float waitTime = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            if (currentOpen)
            {
                SpawnNPC();
            }
        }
    }

    //when the shop close force all NPCs to leave without purchasing anything.
    private void CloseShop()
    {
        for (int i = 0; i < activeNPCs.Count; i++)
        {
            activeNPCs[i].LeaveAbruptly();
        }
    }

    private void SpawnNPC()
    {
        // Safety checks
        if (NPCs == null || NPCs.Count == 0)
        {
            Debug.LogWarning("NPC list is empty!");
            return;
        }

        // Pick a random NPC prefab from the list
        int randomIndex = UnityEngine.Random.Range(0, NPCs.Count);
        GameObject npcPrefab = NPCs[randomIndex];

        if (npcPrefab == null)
        {
            Debug.LogWarning("NPC prefab at index " + randomIndex + " is null!");
            return;
        }

        // Determine spawn position
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;

        // Instantiate NPC and set parent
        GameObject npcInstance = Instantiate(npcPrefab, spawnPosition, Quaternion.identity, parentObject);
        NPC_Controller npcController = npcInstance.GetComponent<NPC_Controller>();

        if (npcController != null)
            activeNPCs.Add(npcController);

        // Optional: reset local position if you want it exactly at parent's position
        // npcInstance.transform.localPosition = Vector3.zero;
    }
}
