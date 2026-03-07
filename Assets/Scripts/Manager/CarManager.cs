using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    public GameObject prefab;
    public Transform spawnPoint;

    private void Start()
    {
        ScheduleNextSpawn();
    }

    private void ScheduleNextSpawn()
    {
        float delay = Random.Range(5f, 10f);
        Invoke(nameof(SpawnObject), delay);
    }

    private void SpawnObject()
    {
        Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        ScheduleNextSpawn();
    }
}
