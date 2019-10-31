using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
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

    public Node parent;

    public Node(bool _walkable, Vector3 _wp, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _wp;
        gridX = _gridX;
        gridY = _gridY;
    }
}
