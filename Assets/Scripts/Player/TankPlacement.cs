using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//currently only spawns tanks via raycast line (would like it to be smoother)...
public class TankPlacement : MonoBehaviour
{

    public GameObject tankObj;
    public PlayerMovement player;

    //will need to change when tank types change
    private bool isHoldingTank = false;

    public Node[] allNodes;

    void Start()
    {
        allNodes = FindObjectsOfType<Node>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //holding tank - should place down tank.
            //need to check if placing down tank will block an exit or register.
            //we can check if any NPC is blocked from reigster and exit OR if the register or exit is blocked from each other
            //Obviously if we check based on every NPC - this would make a lot of computing power
            if(isHoldingTank){
                Spawn();
            }
            //No tank holding - should pick up
            else { 
                DeSpawn(player.checkForTank());
            }  
        }
    }

    private void updateGrid()
    {
        for(int i = 0; i < allNodes.Length; i++)
        {
            allNodes[i].resetNodeConnections();
        }
    }

    //Depsawns the tank if there is a tank there. After despawning switches the holding variable and updates the Node grid.
    public void DeSpawn(GameObject Tank)
    {
        if (Tank != null)
        {
            Destroy(Tank);
            isHoldingTank = true;
            updateGrid();
        }
    }

    //Spawns the tank if there is nothing there. After updates the holding varaible and updates the node grid.
    public void Spawn()
    {
        //rounding the players position to solid numbers.
        float y_pos = Mathf.Ceil(player.transform.position.y);
        float x_pos = Mathf.Ceil(player.transform.position.x);

        //only spawn when there is nothing in the arranged placed area
        if (player.checkForTank() == null)
        {
            isHoldingTank = false;
            //spawns the tank in the direction that the player is facing and in the correct area (one block in front)
            //Need to ensure that tank does not spawn on other objects

            //perhaps clean up, enabled false or true allows ontriggerneter function to occur
            if (player.getDirection() == "up")
            {
                GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos - 0.5f, y_pos + 0.5f, 0), Quaternion.identity);
                tank.GetComponent<BoxCollider2D>().enabled = false;
                tank.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (player.getDirection() == "down")
            {
                GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos - 0.5f, y_pos - 1.5f, 0), Quaternion.identity);
                tank.GetComponent<BoxCollider2D>().enabled = false;
                tank.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (player.getDirection() == "right")
            {
                GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos + 0.5f, y_pos - 0.5f, 0), Quaternion.identity);
                tank.GetComponent<BoxCollider2D>().enabled = false;
                tank.GetComponent<BoxCollider2D>().enabled = true;
            }
            else
            {
                GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos - 1.5f, y_pos - 0.5f, 0), Quaternion.identity);
                tank.GetComponent<BoxCollider2D>().enabled = false;
                tank.GetComponent<BoxCollider2D>().enabled = true;
            }
            updateGrid();
        }

        
    }
}
