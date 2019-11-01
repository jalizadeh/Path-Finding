using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable; // is this cell free to move in
    public Vector3 worldPosition; //the node position in 3D world

    public int gridX;
    public int gridY;

    public int gCost; // distance from starting node
    public int hCost; // (heuristic) distance from end node

    public int fCost //sum of G and H costs
    {
        get
        {
            return gCost + hCost;
        }
    }

    int heapIndex;

    public Node parent;

    public Node(bool _walkable, Vector3 _wp, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _wp;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int HeapIndex
    {
        get {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    //The result is negated, because in this project, the lower is F, it is more valuable for us
    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    
}
