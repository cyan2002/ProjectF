using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCspawning : MonoBehaviour
{
    public GameObject[] NPC;
    public GameObject entrance;
    public GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("a"))
        {
            SpawnNPC();
        }
    }

    //spawns an NPC object at the entrance.
    public void SpawnNPC()
    {
        int num = Random.Range(0, NPC.Length);
        GameObject npc = (GameObject)Instantiate(NPC[num], entrance.transform.position, Quaternion.identity, parent.transform);
    }
}
