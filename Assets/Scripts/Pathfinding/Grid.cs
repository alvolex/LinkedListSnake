using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;
    [SerializeField] LayerMask unwalkableMask;

    [SerializeField] private GameObject floorPrefab;

    private Node[,] grid;
    [SerializeField] private List<Node> path;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    public List<Node> Path
    {
        get => path;
        set => path = value;
    }

    private void Start()
    {
        nodeDiameter = nodeRadius /* * 2*/;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft =
            (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = new Vector2(worldBottomLeft.x + (x * nodeDiameter * nodeRadius), worldBottomLeft.y + (y * nodeDiameter * nodeRadius));
                worldPoint = new Vector2(worldPoint.x + 0.5f, worldPoint.y + 0.5f);

                grid[x, y] = new Node(true, worldPoint, x, y);
                Instantiate(floorPrefab, worldPoint, Quaternion.identity);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();
        
        //Add left neighbour
        if (node.GridX - 1 >= 0 )
        {
            neighbours.Add(grid[node.GridX - 1, node.GridY]);
        }
        //Add right neighbor
        if (node.GridX + 1 < gridSizeX )
        {
            neighbours.Add(grid[node.GridX + 1, node.GridY]);
        }
        //Add bottom neighbour
        if (node.GridY - 1 >= 0 )
        {
            neighbours.Add(grid[node.GridX, node.GridY - 1]);
        }
        //Add top neighbor
        if (node.GridY + 1 < gridSizeY )
        {
            neighbours.Add(grid[node.GridX, node.GridY + 1]);
        }

        return neighbours;
    }

    public Node NodeFromWorldPos(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt(((gridSizeX - 1) * percentX));
        int y = Mathf.RoundToInt(((gridSizeY - 1) * percentY));

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1 ));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.Walkable ? Color.green : Color.red;

                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.gray;
                    }
                }
                Gizmos.DrawCube(n.WorldPos, Vector3.one * (nodeDiameter-.1f));   
            }
        }
        
    }
}
