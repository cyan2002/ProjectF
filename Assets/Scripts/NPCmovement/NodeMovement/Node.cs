using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node cameFrom;
    public List<Node> connections;

    public float gScore;
    public float hScore;
    private bool NodePresent = false;
    private bool ObjectPresent = false;
    private bool onObject = false;

    private Rigidbody2D rb2d;
    private RaycastHit2D[] hit;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    //The start method creates 4 arrays of raycasts coming from the node in each direction. They each check to see if there
    //are other nodes in the area and assign them to their connections list.
    //These raycasts do not detect their own object due to a setting changed in the project settings > Physics2D > "Queries Start In Collider".
    private void Start()
    {
        if (!onObject)
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.up, .75f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, -Vector2.up, .75f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, Vector2.right, .75f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, -Vector2.right, .75f);
            CheckForNodeOrObject();
        } 
    }

    //resets the node connections whenever there is a new change to the map (place a new tank or getting more area unlocked. 
    public void resetNodeConnections()
    {
        connections.Clear();
        Start();
    }

    private void CheckForNodeOrObject()
    {
        int num = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider.CompareTag("Object"))
            {
                ObjectPresent = true;
            }
            if (hit[i].collider.CompareTag("Node"))
            {
                NodePresent = true;
            }
            num = i;
        }

        if (!ObjectPresent && NodePresent)
        {
            connections.Add(hit[num].collider.gameObject.GetComponent<Node>());
        }

        NodePresent = false;
        ObjectPresent = false;
    }

    //F Score is the algorithim to determine where NPCs will move. 
    public float FScore()
    {
        return gScore + hScore;
    }

    //draws connection lines between nodes for error checking. 
    private void OnDrawGizmos()
    {
        if (connections.Count > 0)
        {
            Gizmos.color = Color.blue;
            for (int i = 0; i < connections.Count; i++)
            {
                Gizmos.DrawLine(transform.position, connections[i].transform.position);
            }
        }
    }

    //Is called whenever a tank is placed on that specific node
    //when a tank is placed - remove all current connections that node contains. 
    //Issue is it only places or is called when a Collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Object"))
        {
            connections.Clear();
            onObject = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Object"))
        {
            onObject = false;
        }
    }
}