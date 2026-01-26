using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//fix issue with making it to register then going to checkout...
public class NPC_Controller : MonoBehaviour
{
    [SerializeField]
    private Node currentNode;
    public List<Node> path = new List<Node>();
    private Node StoreDoor;
    private Node LineEntry;
    public Node TargetSpot;

    [SerializeField]
    private float stayTime;
    [SerializeField]
    private float decideTime;
    [SerializeField]
    private float lineTime = 30f;

    //pause timer variables
    private float pauseCheckTimer = 0f;
    private float nextPauseTime;
    private float pauseDuration;
    private float pauseTimer;
    private bool isPaused;

    public bool hasJoinedLine = false;

    [SerializeField]
    List<InventoryItem> itemsToBuy = new List<InventoryItem>();

    [SerializeField]
    List<Transform> viewedShelves = new List<Transform>();

    public float speed = 1f;

    //different states of the NPCs, spawn in as shopping
    public enum NPCState
    {
        Shopping,
        GoingToLineEntry,
        InLine,
        MoveInLine,
        CheckingOut,
        LeavingStore,
        Idle
    }


    public NPCState currentState;
    public NPCState savedState;

    private void Start()
    {
        StoreDoor = GameObject.Find("StartNode").GetComponent<Node>();
        currentNode = GameObject.Find("StartNode").GetComponent<Node>();
        LineEntry = GameObject.Find("LineEntry").GetComponent<Node>();

        if(StoreDoor == null || currentNode == null || LineEntry == null)
        {
            Debug.Log("one of the assigned nodes are null - error!");
        }

        ScheduleNextPause();
        stayTime = UnityEngine.Random.Range(90f, 120f);
        decideTime = UnityEngine.Random.Range(50f, 70f);
    }

    void ScheduleNextPause()
    {
        nextPauseTime = UnityEngine.Random.Range(5f, 20f);   // when to pause
        pauseDuration = UnityEngine.Random.Range(5f, 7f);    // how long to pause
    }

    private void Update()
    {
        switch (currentState)
        {
            //Triggered by timer
            case NPCState.Idle:
                // Play idle animation, stop movement
                break;

            //NPC spawns as this state
            case NPCState.Shopping:
                CreatePath();
                break;

            //triggered by timer
            case NPCState.GoingToLineEntry:
                HeadTarget(LineEntry);
                break;

            //changed by finishing the GoingToLineEntry movement path
            case NPCState.InLine:
                //play animation?
                break;

            //this is changed from the ShopLine script
            case NPCState.MoveInLine:
                HeadTarget(TargetSpot);
                break;

            //Triggered by shopline script - user input "e"
            case NPCState.CheckingOut:
                HandleCheckout();
                break;

            //after checking out this triggers
            case NPCState.LeavingStore:
                HeadTarget(StoreDoor);
                break;
        }

        HandleTimers();        // impatience, random pauses, etc
        HandleRandomPause();
    }

    //called in shopline when E is pressed
    public void MoveInLineStateChange()
    {
        ChangeState(NPCState.MoveInLine);
    }

    //checking out, selling items and leaving line
    //goes through the saved list of all the viewed shelves and purchases the item by adding money to the balance and removing the item.
    public void HandleCheckout()
    {
        foreach (InventoryItem item in itemsToBuy)
        {
            ItemGrid grid = item.GetComponentInParent<ItemGrid>(true);
            if (grid != null)
            {
                int cost = item.itemData.sellcost;
                MoneyManager.Instance.AddMoney(cost);
                grid.RemoveItem(item);
                Destroy(item.gameObject);
            }
            else
            {
                Debug.Log("error!");
            }
        }
        ChangeState(NPCState.LeavingStore);
    }

    //dealing with timer things.
    void HandleTimers()
    {
        // Only decrement timers if NPC is shopping
        if (currentState == NPCState.Shopping)
        {
            stayTime -= Time.deltaTime;
            decideTime -= Time.deltaTime;

            if (decideTime <= 0)
            {
                if (ShopLine.Instance.CheckLineFull())
                {
                    ChangeState(NPCState.LeavingStore);
                }
                else if (itemsToBuy.Count == 0)
                {
                    ChangeState(NPCState.LeavingStore);
                }
                else
                {
                    ChangeState(NPCState.GoingToLineEntry);
                }
            }

            if (stayTime <= 0)
            {
                LeaveAbruptly();
            }
        }

        if (currentState == NPCState.InLine || currentState == NPCState.MoveInLine)
        {
            lineTime -= Time.deltaTime;

            if (lineTime <= 0)
            {
                LeaveAbruptly();
            }
        }
    }

    void LeaveAbruptly()
    {
        ChangeState(NPCState.LeavingStore);
        //run this only when NPC is leaving abruptly due to impatience.
        ShopLine.Instance.LeaveLine(this);
        //need to remove items from list and reset reservered status.

        foreach (InventoryItem item in itemsToBuy)
        {
            item.isTaken = false;
        }

        itemsToBuy.Clear();
    }

    void HandleRandomPause()
    {
        // Don't pause while checking out or leaving
        if (currentState != NPCState.Shopping && currentState != NPCState.Idle)
            return;

        if (!isPaused)
        {
            pauseCheckTimer += Time.deltaTime;

            if (pauseCheckTimer >= nextPauseTime)
            {
                EnterPause();
            }
        }
        else
        {
            pauseTimer += Time.deltaTime;

            if (pauseTimer >= pauseDuration)
            {
                ExitPause();
            }
        }
    }

    void EnterPause()
    {
        isPaused = true;
        savedState = currentState;
        currentState = NPCState.Idle;

        pauseTimer = 0f;
    }

    void ExitPause()
    {
        isPaused = false;
        savedState = NPCState.Shopping;
        currentState = NPCState.Shopping;

        pauseCheckTimer = 0f;
        ScheduleNextPause();
    }

    private void ChangeState(NPCState newState)
    {
        //if the new state to be changed is the same one don't start a new state
        if (savedState == newState) return;

        currentState = newState;
        savedState = newState;

        OnEnterState(newState);
    }

    void OnEnterState(NPCState state)
    {
        switch (state)
        {
            case NPCState.LeavingStore:
                SetPath(StoreDoor);   // runs ONCE
                break;

            case NPCState.GoingToLineEntry:
                SetPath(LineEntry);
                break;

            case NPCState.InLine:
                // maybe play wait animation
                break;
        }
    }



    //this function heads towards the dictated path set by the AStar script. When the path is completed is creates a new randomized path.
    public void CreatePath()
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.05f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            Node[] nodes = FindObjectsOfType<Node>();
            while (path == null || path.Count == 0)
            {
                //this is where we create a new randomized path when the old one has finished.
                //where you can decide where to take a break or head to the register. 
                path = AStarManager.instance.GeneratePath(currentNode, nodes[UnityEngine.Random.Range(0, nodes.Length)]);
            }
        }
    }

    //Called when the map changes and the NPC must recalculate it's path
    public void RecalculatePath()
    {
        Node[] nodes = FindObjectsOfType<Node>();
        path = AStarManager.instance.GeneratePath(currentNode, nodes[UnityEngine.Random.Range(0, nodes.Length)]);
        CreatePath();
    }

    //function to leave the store. If the NPC is currently in the middle of the route, it will finish it's route and head to exit thereafter. 
    public void HeadTarget(Node target)
    {
        if (path.Count > 0)
        {
            int x = 0;
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(path[x].transform.position.x, path[x].transform.position.y, -2), speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, path[x].transform.position) < 0.05f)
            {
                currentNode = path[x];
                path.RemoveAt(x);
            }
        }
        else
        {
            OnReachedTarget();
        }
    }

    private void OnReachedTarget()
    {
        if (currentState == NPCState.GoingToLineEntry)
        {
            // Only join the line once
            if (!hasJoinedLine)
            {
                TargetSpot = ShopLine.Instance.JoinLine(this);
                hasJoinedLine = true;  // prevents multiple calls
                SetPath(TargetSpot);
            }

            // Once path to the line spot is complete
            if (path.Count == 0)
            {
                ChangeState(NPCState.InLine);
            }
        }
        if (currentState == NPCState.MoveInLine) //plays when moving up in line and only after NPC has reached count = 0, completed its path
        {
            ChangeState(NPCState.InLine);
        }
        if (currentState == NPCState.LeavingStore)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetPath(Node target)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        path = AStarManager.instance.GeneratePath(currentNode, target);
        TargetSpot = target;
    }

    //if the NPC hits a object collider recalcuate another path
    //only should play when changing or editing object positions (tanks and shelfs)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        RecalculatePath();
    }

    //detection range for if the NPC wants to buy something/
    //If the NPC wants to buy something, it deletes the object directly from the Grid and adds money to the player's balance.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(currentState == NPCState.LeavingStore || currentState == NPCState.CheckingOut){ return; }
        if (collision.gameObject.GetComponent<ShelfInventoryToggle>() == null)
        {
            return;
        }

        GameObject stand = collision.gameObject.GetComponent<ShelfInventoryToggle>().controlledGrid;

        if(!viewedShelves.Contains(stand.transform))
        {
            viewedShelves.Add(stand.transform);
        }

        TraverseChildren(stand.transform);
    }

    //itmes go fast perhaps adding something where NPCs can only view a shelfing once before leaving...?
    //create item trait that says if its been reservered by another NPC or not. Then make final purchase/transaction at cashier.
    private void TraverseChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ItemGrid grid = child.GetComponent<ItemGrid>(); // ✅ DECLARED HERE

            if (grid == null)
            {
                Debug.LogError("ItemGrid missing on child: " + child.name);
                continue;
            }

            foreach (Transform grandchild in child)
            {
                if (UnityEngine.Random.value < 0.1f)
                {
                    InventoryItem item = grandchild.GetComponent<InventoryItem>();
                    if (item != null && !item.isTaken)
                    {
                        item.isTaken = true;
                        itemsToBuy.Add(item);
                    }
                }
            }
        }
    }

}