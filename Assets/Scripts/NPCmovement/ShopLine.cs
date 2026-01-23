using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//create a list of Nodes that hold the line for the NPCs
//holds next available Node in line that each NPC can access. 
//this will be a singleton so that all NPCs can access the shop line data freely
//if the line is full, the customer will get angry and leave.
//when customer decides to leave, it will empty all the reservered items back into the store
public class ShopLine : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
