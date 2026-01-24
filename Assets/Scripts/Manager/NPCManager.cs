using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//handles spawning NPCs and line management for check out
public class NPCManager : MonoBehaviour
{
    public List<GameObject> NPCs = new List<GameObject>();
    public Transform spawnPoint;
    public Transform parentObject;

    public float minSpawnTime = 5f;
    public float maxSpawnTime = 10f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true) // keeps running forever
        {
            // Wait a random interval
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            SpawnNPC();
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
        int randomIndex = Random.Range(0, NPCs.Count);
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

        // Optional: reset local position if you want it exactly at parent's position
        // npcInstance.transform.localPosition = Vector3.zero;
    }
}
