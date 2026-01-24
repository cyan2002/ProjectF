using System;
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
    public static ShopLine Instance { get; private set; }
    //assign manually for now
    public List<Node> line = new List<Node>();
    //cap on the line
    private int lineCap;

    [SerializeField]
    private Queue<NPC_Controller> npcQueue = new Queue<NPC_Controller>();

    private bool isNear = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // only allow one instance
            return;
        }
        Instance = this;
        lineCap = line.Count;
        PlayerInput.HandleE += HandleTransaction;
    }

    //when the player presses E and they are in range, handle transaction with customer
    //need something to put in so it doesn't go too fast.
    private void HandleTransaction()
    {
        if (isNear)
        {
            if (npcQueue.Count == 0)
            return;

        NPC_Controller frontNPC = npcQueue.Peek();
        frontNPC.HandleCheckout();
        LeaveLine(frontNPC);

        // Update everyone�s target node
        int i = 0;
        foreach (var npc in npcQueue)
        {
            Node newTarget = line[Mathf.Min(i, line.Count - 1)];
            npc.SetPath(newTarget);
            npc.MoveInLineStateChange();
            i++;
        }
        }
    }

    //adds next NPC line and returns the next node in line, use this as a target for the NPC
    //returns null if the line is full
    public Node GetNextNodeInLine(NPC_Controller npc)
    {
        if (npcQueue.Count >= line.Count)
        {
            return null;
        }

        npcQueue.Enqueue(npc);
        int position = npcQueue.Count - 1;
        if (position >= line.Count)
            position = line.Count - 1;
        return line[position];
    }

    //gets called either when they run out of patience (waited too long in line)\
    //or after purchase has been made
    //create a new queue with all NPCs that are NOT leaving because queue does not remove points in the middle
    public void LeaveLine(NPC_Controller npc)
    {
        Queue<NPC_Controller> newQueue = new Queue<NPC_Controller>();
        foreach (var n in npcQueue)
        {
            if (n != npc) newQueue.Enqueue(n);
        }
        npcQueue = newQueue;
    }

    public bool IsFront(NPC_Controller npc)
    {
        return npcQueue.Count > 0 && npcQueue.Peek() == npc;
    }

    public Node JoinLine(NPC_Controller npc)
    {
        if (npc.hasJoinedLine) return npc.TargetSpot;

        npc.hasJoinedLine = true;
        npcQueue.Enqueue(npc);

        int index = npcQueue.Count - 1;
        Node targetNode = line[Mathf.Min(index, line.Count - 1)];

        return targetNode;
    }

    //returns true if the shopping line is full
    public bool CheckLineFull()
    {
        return npcQueue.Count == line.Count;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isNear = true;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            isNear = false;
        }
    }
}
