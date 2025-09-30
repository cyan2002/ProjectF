using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

//Script it to access the different nodes of the A* map and then select one at random for the NPC to pathfind to
public class NodeSelection : MonoBehaviour
{
    GridGraph gg = AstarPath.active.data.gridGraph;
    gg.GetNodes(node => {
    // Here is a node
    Debug.Log("I found a node at position " + (Vector3) node.position);
});

}
