using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankPlacement : MonoBehaviour
{

    public GameObject tankObj;
    public GameObject player;

    //will need to change when tank types change
    private bool isHoldingTank = false;

    public Node[] allNodes;

    // Start is called before the first frame update
    void Start()
    {
        allNodes = FindObjectsOfType<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        //detects if the space key is pressed and thus place tank
        if (Input.GetKeyDown("space"))
        {
            //Need to update the Grid for NPC movement
            
            //holding tank - should place down tank. NEED TO FIX - can't place down tank on same square as another tank
            if(isHoldingTank){
                Spawn();
                updateGrid();
                //should be if suceed change variable to not holding
                isHoldingTank = false;
            }
            //No tank holding - should pick up
            else{
                //checks for if a tank is in the way. If so it picks it up
                if(player.GetComponent<PlayerMovement>().checkForTank()){
                    isHoldingTank = true;
                    updateGrid();
                }
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

    //Spawns the tank
    public void Spawn()
    {
        //rounding the players position to solid numbers.
        float y_pos = Mathf.Ceil(player.transform.position.y);
        float x_pos = Mathf.Ceil(player.transform.position.x);

        //spawns the tank in the direction that the player is facing and in the correct area (one block in front)
        //Need to ensure that tank does not spawn on other objects
        if(player.GetComponent<PlayerMovement>().getDirection() == "up")
        {
            GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos-0.5f, y_pos+0.5f, 0), Quaternion.identity);
        }
        else if(player.GetComponent<PlayerMovement>().getDirection() == "down")
        {
            GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos-0.5f, y_pos-1.5f, 0), Quaternion.identity);
        }
        else if(player.GetComponent<PlayerMovement>().getDirection() == "right")
        {
            GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos+0.5f, y_pos-0.5f, 0), Quaternion.identity);
        }
        else
        {
            GameObject tank = (GameObject)Instantiate(tankObj, new Vector3(x_pos-1.5f, y_pos-0.5f, 0), Quaternion.identity);
        }
        
    }
}
