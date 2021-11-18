using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AbstractDataTypes;
using UnityEngine;
using UnityEngine.Events;

public class SnakeMovementAstar : MonoBehaviour
{
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private float timeBetweenMoves = 0.3f;
    [SerializeField] private float minTimeBetweenMoves = 0.05f;

    //Used for tail to follow snake head (Hidden because we progrematically access these)
    [HideInInspector] public UnityEvent SnakeHasMoved; 
    private Vector3 lastPos;
    private Vector3 tailLastPos;

    private Transform snakeHeadMover;

    private Direction startDirection = Direction.Right; 
    private Direction currentDirection;
    private Direction lastDirection;
    private Vector3 continueLastMovement;

    [SerializeField]private List<Vector2> pathToFollow = new List<Vector2>();

    public MyLinkedList<GameObject> mySnakeLL = new MyLinkedList<GameObject>();

    public Vector3 TailLastPos => tailLastPos;

    private enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }

    private void Awake()
    {
        FoodEaten.OnFoodEaten += UpdateSnakeSpeed;
        AStar.OnPathFound += UpdateSnakePath;
    }

    void Start()
    {
        MyListNode<GameObject> myHeadNode = new MyListNode<GameObject>(this.gameObject);
        mySnakeLL.AddLast(myHeadNode);

        snakeHeadMover = snakeHead.transform;
        currentDirection = startDirection;
        StartCoroutine(MoveSnake());
    }

    private void OnDestroy()
    {
        FoodEaten.OnFoodEaten -= UpdateSnakeSpeed;
        AStar.OnPathFound -= UpdateSnakePath;
    }

    private void UpdateSnakeSpeed()
    {
        if (timeBetweenMoves <= minTimeBetweenMoves) return;
        timeBetweenMoves -= 0.005f;
    }
    
    private void UpdateSnakePath(List<Vector2> path)
    {
        pathToFollow = path;
    }
    
    IEnumerator MoveSnake()
    {
        while (true)
        {
            if (pathToFollow.Count > 0)
            {
                snakeHeadMover.position = pathToFollow[0];
                pathToFollow.RemoveAt(0);
                
                //todo make snake go to food instead and then add all tail positions to the non-walkable gridset
                
                
                lastPos = snakeHeadMover.position; //Save a reference to our position before we move. Used to update tail.
                UpdateTailPositions(); //Update all the tail positions using our linked list implementation
                SnakeHasMoved.Invoke(); //Tells our tail that we have moved
            }
            
            yield return new WaitForSeconds(timeBetweenMoves);
        }
    }

    private void UpdateTailPositions()
    {
        MyListNode<GameObject> curNode = mySnakeLL.First();

        while (curNode != null)
        {
            if (curNode == mySnakeLL.First() && curNode.next != null)
            {
                TailPiece nextPiece = curNode.next.data.GetComponent<TailPiece>();
                nextPiece.LastPos = nextPiece.transform.position;
                curNode.next.data.transform.position = lastPos;
            }
            else if (curNode.next != null)
            {
                TailPiece nextPiece = curNode.next.data.GetComponent<TailPiece>(); //Get ref to next piece
                nextPiece.LastPos = nextPiece.transform.position; //Set next pieces last pos to be it's current pos
                nextPiece.transform.position = curNode.data.GetComponent<TailPiece>().LastPos; // Update the tail nodes current pos
            }
            curNode = curNode.next;
        }
        
        //Keep a reference to where we should add the next tail piece
        if (mySnakeLL.Last().data.TryGetComponent<TailPiece>(out TailPiece tp))
        {
            tailLastPos = tp.LastPos;
        }
        else
        {
            tailLastPos = lastPos;
        }
    }
}
