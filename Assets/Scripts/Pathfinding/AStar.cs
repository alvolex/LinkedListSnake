using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [SerializeField] private List<Vector2> currentPath = new List<Vector2>();
    public static event Action<List<Vector2>> OnPathFound;

    private Vector2 ifTargetIsUnreachableMoveHere = new Vector2(14.5f, 9.5f);
    
    public Transform seeker/*, target*/;
    public Vector2 target;
    Grid grid;

    void Awake()
    {
        Grid.OnGridCreated += HandleGridCreated;
    }

    private void Start()
    {
        grid = GetComponent<Grid>();
        target = GetComponent<Grid>().FoodPos;
        FindPath (seeker.position, target);
    }

    private void OnDestroy()
    {
        Grid.OnGridCreated -= HandleGridCreated;
    }

    private void HandleGridCreated()
    {
        grid = GetComponent<Grid>();
        target = GetComponent<Grid>().FoodPos;

        /*if (targetUnreachable)
        {
            //FindPath(seeker.position, ifTargetIsUnreachableMoveHere);
            OnGameOver?.Invoke();
            return;
        }*/
        FindPath (seeker.position, target);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos) {
        Node startNode = grid.NodeFromWorldPos(startPos);
        Node targetNode = grid.NodeFromWorldPos(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0) {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i ++) {
                if (openSet[i].FCost < node.FCost || openSet[i].FCost == node.FCost) {
                    if (openSet[i].HCost < node.HCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == targetNode) {                
                RetracePath(startNode,targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node)) {
                if (!neighbour.Walkable || closedSet.Contains(neighbour)) {
                    continue;
                }

                int newCostToNeighbour = node.GCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour)) {
                    neighbour.GCost = newCostToNeighbour;
                    neighbour.HCost = GetDistance(neighbour, targetNode);
                    neighbour.Parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        currentPath.Clear();

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }
        path.Reverse();

        foreach (Node node in path)
        {
            currentPath.Add(new Vector2(node.GridX - 13.5f, node.GridY - 7.5f));
        }

        OnPathFound?.Invoke(currentPath);
        grid.Path = path;
    }

    int GetDistance(Node nodeA, Node nodeB) {
        int dstX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int dstY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        if (dstX > dstY)
            return 14*dstY + 10* (dstX-dstY);
        return 14*dstX + 10 * (dstY-dstX);
    }
}
