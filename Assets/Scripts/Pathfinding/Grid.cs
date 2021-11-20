using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;


public class Grid : MonoBehaviour
{
    [SerializeField] private SnakeMovementAstar player;
    
    [SerializeField] Vector2 gridWorldSize;
    [SerializeField] float nodeRadius;
    [SerializeField] private GameObject gridParent;

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject foodPrefab;
    
    public static event Action OnGridCreated;
    
    private Node[,] grid;
    [SerializeField] private List<Node> path;

    private Vector2 foodPos;

    private float nodeDiameter;
    private int gridSizeX, gridSizeY;
    private bool shouldInstantiateGrid = true;

    private List<Vector2> positionsToSpawnFoodOn = new List<Vector2>();

    //Getters
    public Vector2 FoodPos => foodPos;
    public List<Node> Path
    {
        get => path;
        set => path = value;
    }

    private void Awake()
    {
        Time.timeScale = 1f;
        nodeDiameter = nodeRadius /* * 2*/;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
        FoodEaten.OnFoodEaten += CreateGrid;
    }

    private void OnDestroy()
    {
        FoodEaten.OnFoodEaten -= CreateGrid;
    }

    private void CreateGrid()
    {
        positionsToSpawnFoodOn = new List<Vector2>(); //Reset list so we don't store old positions
        
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft =
            (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        List<Vector2> tailPositions = player.AllTailPositions;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = new Vector2(worldBottomLeft.x + (x * nodeDiameter * nodeRadius), worldBottomLeft.y + (y * nodeDiameter * nodeRadius));
                worldPoint = new Vector2(worldPoint.x + 0.5f, worldPoint.y + 0.5f);

                if (tailPositions !=null && tailPositions.Contains(worldPoint))
                {
                    grid[x, y] = new Node(false, worldPoint, x, y);
                }
                else
                {
                    grid[x, y] = new Node(true, worldPoint, x, y);
                    
                    if (!((x == 0 || x == gridSizeX - 1) || (y == 0 || y == gridSizeY -1)))
                    {
                        positionsToSpawnFoodOn.Add(worldPoint); // *Cheating* a little bit by never adding food to the edges of the map.
                    }
                }

                if (shouldInstantiateGrid)
                {
                    Instantiate(floorPrefab, worldPoint, Quaternion.identity, gridParent.transform);
                }
            }
        }
        shouldInstantiateGrid = false;

        int getRandomPointToSpawnFoodOn = Random.Range(0, positionsToSpawnFoodOn.Count - 1);
        Instantiate(foodPrefab, positionsToSpawnFoodOn[getRandomPointToSpawnFoodOn], Quaternion.identity);
        foodPos = positionsToSpawnFoodOn[getRandomPointToSpawnFoodOn];

        OnGridCreated?.Invoke();
    }
    
    public void RecalculateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft =
            (Vector2)transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

        List<Vector2> tailPositions = player.AllTailPositions;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = new Vector2(worldBottomLeft.x + (x * nodeDiameter * nodeRadius), worldBottomLeft.y + (y * nodeDiameter * nodeRadius));
                worldPoint = new Vector2(worldPoint.x + 0.5f, worldPoint.y + 0.5f);

                if (tailPositions !=null && tailPositions.Contains(worldPoint))
                {
                    grid[x, y] = new Node(false, worldPoint, x, y);
                }
                else
                {
                    grid[x, y] = new Node(true, worldPoint, x, y);
                }
            }
        }
        OnGridCreated?.Invoke();
    }

    public bool CheckIfWalkable(Vector2 pos)
    {
        Node nodeToCheck = NodeFromWorldPos(pos);
        
        if (nodeToCheck == null || pos.x < -14 || pos.x > 14 || pos.y < -8 || pos.y > 8 )
        {
            return false;
        }
        
        return nodeToCheck.Walkable;
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
