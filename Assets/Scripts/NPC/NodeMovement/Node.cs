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

    [SerializeField]
    private bool tester;

    private Rigidbody2D rb2d;
    private RaycastHit2D[] hit;

    //The start method creates 4 arrays of raycasts coming from the node in each direction. They each check to see if there
    //are other nodes in the area and assign them to their connections list.
    //These raycasts do not detect their own object due to a setting changed in the project settings > Physics2D > "Queries Start In Collider".
    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (!onObject)
        {
            hit = Physics2D.RaycastAll(transform.position, Vector2.up, .35f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, -Vector2.up, .35f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, Vector2.right, .35f);
            CheckForNodeOrObject();

            hit = Physics2D.RaycastAll(transform.position, -Vector2.right, .35f);
            CheckForNodeOrObject();
        } 
    }

    //resets the node connections whenever there is a new change to the map (place a new tank or getting more area unlocked. 
    public void resetNodeConnections()
    {
        connections.Clear();
        Awake();
    }

    private void CheckForNodeOrObject()
    {
        if (hit.Length == 0) return; // no colliders found, nothing to do
        
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

        if (hit[num].collider.gameObject.GetComponent<Node>() == null)
        {
            return;
        }

        if (!ObjectPresent && NodePresent)
        {
            connections.Add(hit[num].collider.gameObject.GetComponent<Node>());
        }

        //needs to be reset if I decide to place and pick up tanks.
        NodePresent = false;
        ObjectPresent = false;

        //object present not as expected...
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
        if (collision.CompareTag("Object"))
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