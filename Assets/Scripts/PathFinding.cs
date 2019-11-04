using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    Grid grid;
    PathRequestManager pathRequestManager;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        pathRequestManager = GetComponent<PathRequestManager>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback) {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {
            //List<Node> openSet = new List<Node>();
            Heap<Node> openSet = new Heap<Node>(grid.MaxGridSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                //if we reached the end, finish now
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found in: " + sw.ElapsedMilliseconds + " ms");

                    pathSuccess = true;

                    //grid.openSet = openSet;
                    break;
                }


                //so it is not finished, go through all neigbours
                foreach (Node neigbour in grid.GetNeigbours(currentNode))
                {
                    if (!neigbour.walkable || closedSet.Contains(neigbour))
                        continue;

                    int newMoveCostToNeigbour = currentNode.gCost + GetDistanceOfTwoNodes(currentNode, neigbour) + neigbour.movementPenalty;
                    if (newMoveCostToNeigbour < neigbour.gCost || !openSet.Contains(neigbour))
                    {
                        //for setting F cost, I have to set G & H
                        neigbour.gCost = newMoveCostToNeigbour;
                        neigbour.hCost = GetDistanceOfTwoNodes(neigbour, targetNode);
                        neigbour.parent = currentNode;

                        if (!openSet.Contains(neigbour))
                        {
                            openSet.Add(neigbour);
                        } else
                        {
                            openSet.UpdateItem(neigbour);
                        }
                    }
                }

            }
        }

        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }

        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    

    private int GetDistanceOfTwoNodes(Node nodeA, Node nodeB)
    {
        int distanceX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distanceY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distanceX > distanceY)
            return (14 * distanceY) + (10 * (distanceX - distanceY));

        return (14 * distanceX) + (10 * (distanceY - distanceX));
    }



    private Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();

        //each node know only his parent, so from the end node, I can trace backward
        //to the start node
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);

        return waypoints;
    }


    //Instead of returning all the positions, this method removes the points in the same direction
    // to simplify the path.
    //NOTE: it doesn't change the path, only removed the same iterated points in a direction and is
    // used only for drawing 
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++){
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }

        return waypoints.ToArray();
    }
}
