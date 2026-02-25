using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager instance;

    private void Awake()
    {
        instance = this;
    }

    //This method returns a list of nodes that is the path from the given start to end node.
    //Returns null if there is no path between the start and end node. 
    //This uses the F score to calculate the best path to the start and end. 
    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>();
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> hScore = new Dictionary<Node, float>();
        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        foreach (Node n in FindObjectsOfType<Node>())
            gScore[n] = float.MaxValue;

        gScore[start] = 0;
        hScore[start] = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                float f = gScore[openSet[i]] + hScore.GetValueOrDefault(openSet[i], 0);
                float currentF = gScore[currentNode] + hScore.GetValueOrDefault(currentNode, 0);
                if (f < currentF) currentNode = openSet[i];
            }

            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();
                path.Add(end);
                while (currentNode != start)
                {
                    currentNode = cameFrom[currentNode];
                    path.Add(currentNode);
                }
                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.connections)
            {
                float tentativeG = gScore[currentNode] + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);
                if (tentativeG < gScore.GetValueOrDefault(connectedNode, float.MaxValue))
                {
                    cameFrom[connectedNode] = currentNode;
                    gScore[connectedNode] = tentativeG;
                    hScore[connectedNode] = Vector2.Distance(connectedNode.transform.position, end.transform.position);
                    if (!openSet.Contains(connectedNode))
                        openSet.Add(connectedNode);
                }
            }
        }
        Debug.Log($"No path found from {start.name} to {end.name}. Explored {cameFrom.Count} nodes.");
    return null;
}

    public Node FindNearestNode(Vector2 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                foundNode = node;
            }
        }
        return foundNode;
    }

    public Node FindFurthestNode(Vector2 pos)
    {
        Node foundNode = null;
        float maxDistance = default;

        foreach (Node node in FindObjectsOfType<Node>())
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node[] AllNodes()
    {
        return FindObjectsOfType<Node>();
    }
}