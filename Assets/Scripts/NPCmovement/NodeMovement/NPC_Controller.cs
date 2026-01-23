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

    //pause timer variables
    private float pauseCheckTimer = 0f;
    private float nextPauseTime;
    private float pauseDuration;
    private float pauseTimer;
    private bool isPaused;

    private bool hasArrived = false;
    public bool hasJoinedLine = false;

    public float speed = 1f;

    //different states of the NPCs, spawn in as shopping
    public enum NPCState
    {
        Shopping,
        GoingToLineEntry,
        InLine,
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
            case NPCState.Idle:
                // Play idle animation, stop movement
                break;

            case NPCState.Shopping:
                CreatePath();
                break;

            case NPCState.GoingToLineEntry:
                HeadTarget(LineEntry);
                break;

            case NPCState.InLine:
                if (!hasArrived)
                {
                    HeadTarget(TargetSpot);  // move to line
                }
                else
                {
                    Debug.Log("hello"); // now standing in line
                                        // wait, idle animation, etc.
                }
                break;

            case NPCState.CheckingOut:
                HandleCheckout();
                break;

            case NPCState.LeavingStore:
                HeadTarget(StoreDoor);
                break;
        }

        HandleTimers();        // impatience, random pauses, etc
        HandleRandomPause();
    }

    //checking out, selling items and leaving line
    private void HandleCheckout()
    {
        throw new NotImplementedException();
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
                ChangeState(NPCState.GoingToLineEntry);
            }

            if (stayTime <= 0)
            {
                ChangeState(NPCState.LeavingStore);
            }
        }
    }

    void HandleRandomPause()
    {
        // Don't pause while checking out or leaving
        if (currentState == NPCState.CheckingOut || currentState == NPCState.LeavingStore)
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

    void ChangeState(NPCState newState)
    {
        
        //if the new state to be changed is the same one don't start a new state
        if (savedState == newState) return;

        Debug.Log(newState);

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
                Debug.Log("1");
                SetPath(LineEntry);
                break;

            case NPCState.InLine:
                Debug.Log("12");
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
            if (!hasArrived)
            {
                OnReachedTarget();
                hasArrived = true;  // <-- prevents multiple calls
            }
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
                hasArrived = true;
            }
        }
    }

    public void SetPath(Node target)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        path = AStarManager.instance.GeneratePath(currentNode, target);
        TargetSpot = target;
        hasArrived = false;  // reset so OnReachedTarget will fire for the new destination
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
        if (collision.gameObject.GetComponent<ShelfInventoryToggle>() == null)
        {
            return;
        }

        GameObject stand = collision.gameObject.GetComponent<ShelfInventoryToggle>().controlledGrid;

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

        var itemsToBuy = new List<InventoryItem>();

        foreach (Transform grandchild in child)
        {
            if (UnityEngine.Random.value < 0.1f)
            {
                InventoryItem item = grandchild.GetComponent<InventoryItem>();
                if (item != null)
                {
                    itemsToBuy.Add(item);
                }
            }
        }

        foreach (InventoryItem item in itemsToBuy)
        {
            int cost = item.itemData.sellcost;
            grid.RemoveItem(item);                // ✅ object-based removal
            MoneyManager.Instance.AddMoney(cost);
            Destroy(item.gameObject);
        }
    }
}

}