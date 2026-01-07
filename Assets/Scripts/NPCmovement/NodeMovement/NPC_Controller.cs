using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//fix issue with making it to register then going to checkout...
public class NPC_Controller : MonoBehaviour
{
    public Node currentNode;
    public List<Node> path = new List<Node>();
    public Node StoreDoor;
    public Node Register;

    public float pauseTimer = 0f;
    private float checkPause;
    private float stayPause;

    private bool pause = false;
    private bool leaving = false;

    //varaible used to  
    private bool once = true;

    public float speed = 1f;

    //tracks which stage NPC is on, browsing, checking out, or leaving
    private int trackCount = 0;

    private void Start()
    {
        StoreDoor = GameObject.Find("StartNode").GetComponent<Node>();
        currentNode = GameObject.Find("StartNode").GetComponent<Node>();
        Register = GameObject.Find("Register").GetComponent<Node>();
        checkPause = Random.Range(5f, 10f);
        stayPause = Random.Range(5f, 10f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            leaving = true;
        }

        //need to decide when to leave for the NPC
        if (leaving)
        {
            if (trackCount == 0)
            {
                if (once)
                {
                    //need edge case of incase path is null
                    once = false;
                    SetPath(Register);
                }
                HeadTarget(Register);
            }
            else if (trackCount == 1)
            {
                if (once)
                {
                    //need edge case incase path is null
                    once = false;
                    SetPath(StoreDoor);
                }
                HeadTarget(StoreDoor);
            }
            else
            {
                print("error");
            }
        }
        else
        {
            timer();
            if (!pause)
            {
                CreatePath();
            }
        }
    }

    //dealing with timer things.
    void timer()
    {
        //timer that is randomized so NPC sometimes takes pauses, anywhere between 5 to 20 seconds it can pause
        //pause can happen on non-grid spaces (between 0.5f)
        if (pauseTimer >= checkPause && !pause)
        {
            pause = true;
            pauseTimer = 0f;
            //plays once per pause
        }
        //if the NPC is already paused, must wait a certain amount of time before moving again.
        else if (pause)
        {
            pauseTimer += Time.deltaTime;
            //only pauses for about 5-7 seconds
            if (pauseTimer >= stayPause)
            {
                checkPause = Random.Range(5f, 20f);
                stayPause = Random.Range(5f, 20f);
                pause = false;
                pauseTimer = 0f;
            }
        }
        else
        {
            pauseTimer += Time.deltaTime;
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
                path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
            }
        }
    }

    //Called when the map changes and the NPC must recalculate it's path
    public void RecalculatePath()
    {
        Node[] nodes = FindObjectsOfType<Node>();
        path = AStarManager.instance.GeneratePath(currentNode, nodes[Random.Range(0, nodes.Length)]);
        CreatePath();
    }

    //function to leave the store. If the NPC is currently in the middle of the route, it will finish it's route and head to exit thereafter. 
    public void HeadTarget(Node target)
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
        else if (trackCount == 0)
        {
            trackCount = 1;
            once = true;
        }
        else if (trackCount == 1)
        {
            //finally reached the end point
            //CHANGE HERE HOW TO HANDLE NPC LEAVING THE STORE
            //print("exit!");
            //Destroy(this.gameObject);
        }
    }

    private void SetPath(Node target)
    {
        Node[] nodes = FindObjectsOfType<Node>();
        path = AStarManager.instance.GeneratePath(currentNode, target);
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
    private void TraverseChildren(Transform parent)
{
    Debug.Log("Deciding on whole selection");

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
            if (Random.value < 0.1f)
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