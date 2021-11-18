using System;
using System.Collections;
using System.Collections.Generic;
using AbstractDataTypes;
using UnityEngine;

public class AddTailPieceAStar : MonoBehaviour
{
    [SerializeField] private SnakeMovementAstar snakeHead;
    [SerializeField] private GameObject tailPrefab;

    private void Awake()
    {
        FoodEaten.OnFoodEaten += AddTail;
    }

    private void OnDestroy()
    {
        FoodEaten.OnFoodEaten -= AddTail;
    }

    private void AddTail()
    {
        GameObject tail = Instantiate(tailPrefab, snakeHead.TailLastPos, Quaternion.identity, snakeHead.transform);
        MyListNode<GameObject> tailNode = new MyListNode<GameObject>(tail);
        snakeHead.mySnakeLL.AddLast(tailNode);
    }
}
