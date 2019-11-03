using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{

    public Transform player;

    Node[,] grid; //2D array of all nodes
    public Vector2 gridWorldSize;
    public float nodeRadius; //half-size of each node's place
    public LayerMask unwalkableMask;

    float nodeDiamater; //whole size of a node place
    int gridSizeX; //number of nodes' places over X
    int gridSizeY; //number of nodes' places over Y

    Vector3 sharedPos;

    public bool displayGridGizmos;

    //use it for creating the heap
    public int MaxGridSize {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void Awake()
    {
        nodeDiamater = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamater);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamater);

        CreateGrid();
    }


    private void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - (Vector3.right * gridSizeX / 2) - (Vector3.forward * gridSizeY / 2);
        sharedPos = worldBottomLeft;

        //or use opposite axis. note that the `-` become `+`, because these axis are already negative
        //Vector3 worldBottomLeft = transform.position + (Vector3.left * gridSizeX / 2) + (Vector3.back * gridSizeY / 2);

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft
                    + Vector3.right * (x * nodeDiamater + nodeRadius)
                    + Vector3.forward * (y * nodeDiamater + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

                /*
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.name = x + "." + y;
                obj.transform.position = worldPoint;
                obj.transform.localScale = Vector3.one;
                */
            }
        }

    }

    //Get the list of all 8 neigbours around the node
    public List<Node> GetNeigbours(Node node)
    {
        List<Node> neigbours = new List<Node>();

        for(int x=-1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //skip the position of the node
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX - x;
                int checkY = node.gridY - y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neigbours.Add(grid[checkX, checkY]);
            }
        }

        return neigbours;
    }

    public Node NodeFromWorldPoint(Vector3 position) {
        float percentX = (position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        /* my solution, but now it doesn't support out of bounds
        int x = Mathf.Abs(Mathf.RoundToInt((-(gridWorldSize.x/2) - position.x) / nodeDiamater));
        int y = Mathf.Abs(Mathf.RoundToInt((-(gridWorldSize.y/2) - position.z) / nodeDiamater));
        print(x + "/" + y);
        */

        return grid[x, y];
    }

    //Draw a wired cube to simulate the whole grid space
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null && displayGridGizmos)
        {
            //Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * nodeDiamater * 0.95f);
            }
        }
    }

}
