using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AbstractDataTypes;
using UnityEngine;
using UnityEngine.Events;

public class SnakeMovement : MonoBehaviour
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

    public MyLinkedList<GameObject> mySnakeLL = new MyLinkedList<GameObject>();

    public Vector3 TailLastPos => tailLastPos;

    private enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }
    void Start()
    {
        MyListNode<GameObject> myHeadNode = new MyListNode<GameObject>(this.gameObject);
        mySnakeLL.AddLast(myHeadNode);

        snakeHeadMover = snakeHead.transform;
        currentDirection = startDirection;
        StartCoroutine(MoveSnake());

        FoodEaten.OnFoodEaten += UpdateSnakeSpeed;
    }

    private void OnDestroy()
    {
        FoodEaten.OnFoodEaten -= UpdateSnakeSpeed;
    }

    private void UpdateSnakeSpeed()
    {
        if (timeBetweenMoves <= minTimeBetweenMoves) return;
        timeBetweenMoves -= 0.005f;
    }

    private void Update()
    {
        HandleMovementInput();
    }

    IEnumerator MoveSnake()
    {
        while (true)
        {
            lastPos = snakeHeadMover.position; //Save a reference to our position before we move. Used to update tail.

            if (currentDirection == Direction.Up && lastDirection != Direction.Down)
            {
                snakeHeadMover.position += transform.up;
                snakeHeadMover.transform.localRotation = Quaternion.Euler(0, 0, 90);
                
                lastDirection = Direction.Up;
                continueLastMovement = transform.up;
            }
            else if (currentDirection == Direction.Down && lastDirection != Direction.Up)
            {
                snakeHeadMover.position -= transform.up;
                snakeHeadMover.transform.localRotation = Quaternion.Euler(0, 0, -90);
                
                lastDirection = Direction.Down;
                continueLastMovement = transform.up * -1;
            }
            else if (currentDirection == Direction.Left && lastDirection != Direction.Right)
            {
                snakeHeadMover.position -= transform.right;
                snakeHeadMover.transform.localRotation = Quaternion.Euler(0, 0, 180);
                
                lastDirection = Direction.Left;
                continueLastMovement = transform.right * -1;
            }
            else if (currentDirection == Direction.Right && lastDirection != Direction.Left)
            {
                snakeHeadMover.position += transform.right;
                snakeHeadMover.transform.localRotation = Quaternion.Euler(0, 0, 0);
                
                lastDirection = Direction.Right;
                continueLastMovement = transform.right;
            }
            else
            {
                snakeHeadMover.position += continueLastMovement;
            }

            UpdateTailPositions(); //Update all the tail positions using our linked list implementation

            SnakeHasMoved.Invoke(); //Tells our tail that we have moved
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

    private void HandleMovementInput()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && currentDirection != Direction.Left)
        {
            currentDirection = Direction.Right;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0 && currentDirection != Direction.Right)
        {
            currentDirection = Direction.Left;
        }
        else if (Input.GetAxisRaw("Vertical") > 0 && currentDirection != Direction.Down)
        {
            currentDirection = Direction.Up;
        }
        else if (Input.GetAxisRaw("Vertical") < 0 && currentDirection != Direction.Up)
        {
            currentDirection = Direction.Down;
        }
    }
}
