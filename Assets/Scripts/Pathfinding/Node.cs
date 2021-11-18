using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    private Node parent;
    
    private bool walkable;
    private Vector2 worldPos;
    private int gridX;
    private int gridY;
    private int gCost, hCost, fCost;

    public Node Parent
    {
        get => parent;
        set => parent = value;
    }

    public Vector2 WorldPos => worldPos;
    public bool Walkable => walkable;

    public int GridX => gridX;
    public int GridY => gridY;

    public int GCost
    {
        get => gCost;
        set => gCost = value;
    }

    public int HCost
    {
        get => hCost;
        set => hCost = value;
    }

    public int FCost => gCost + hCost;

    public Node(bool isWalkable, Vector2 myPos, int _gridX, int _gridY)
    {
        walkable = isWalkable;
        worldPos = myPos;
        gridX = _gridX;
        gridY = _gridY;
    }

}
