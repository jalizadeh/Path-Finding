using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    Grid grid;

    public Transform startNode;
    public Transform targetNode;


    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            FindPath(startNode.position, targetNode.position);
    }


    void FindPath(Vector3 startPos, Vector3 targetPos) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //List<Node> openSet = new List<Node>();
        Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            /* 
             * OLD
             *
            Node currentNode = openSet[0]; //at start, the only node is this

            for(int i =1; i< openSet.Count; i++)
            {
                //replace the current node with node [i]
                // - if it is less expensive
                // - or if only the fCost is the same, but the distance from end node, is less
                if (openSet[i].fCost < currentNode.fCost 
                    || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                        currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            */

            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            //if we reached the end, finish now
            if (currentNode == targetNode)
            {
                sw.Stop();
                print("Path found in: " + sw.ElapsedMilliseconds + " ms");

                //show the path on grid
                RetraceParent(startNode, targetNode);

                //grid.openSet = openSet;
                return;
            }


            //so it is not finished, go through all neigbours
            foreach (Node neigbour in grid.GetNeigbours(currentNode))
            {
                if (!neigbour.walkable || closedSet.Contains(neigbour))
                    continue;

                int newMoveCostToNeigbour = currentNode.gCost + GetDistanceOfTwoNodes(currentNode, neigbour);
                if (newMoveCostToNeigbour < neigbour.gCost || !openSet.Contains(neigbour))
                {
                    //for setting F cost, I have to set G & H
                    neigbour.gCost = newMoveCostToNeigbour;
                    neigbour.hCost = GetDistanceOfTwoNodes(neigbour, targetNode);
                    neigbour.parent = currentNode;

                    if (!openSet.Contains(neigbour))
                    {
                        openSet.Add(neigbour);
                    }
                }
            }

        }
    }


    private int GetDistanceOfTwoNodes(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return (14 * distanceY) + (10 * (distanceX - distanceY));

        return (14 * distanceX) + (10 * (distanceY - distanceX));
    }

    private void RetraceParent(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();

        //each node know only his parent, so from the end node, I can trace backward
        //to the start node
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }
}
