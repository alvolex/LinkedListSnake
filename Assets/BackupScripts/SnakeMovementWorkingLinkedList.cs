using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AbstractDataTypes;
using UnityEngine;
using UnityEngine.Events;

public class SnakeMovementWorkingLinkedList : MonoBehaviour
{
    [SerializeField] private GameObject snakeHead;
    [SerializeField] private float timeBetweenMoves = 0.3f;

    //Used for tail to follow snake head (Hidden because we progrematically access these)
    [HideInInspector] public UnityEvent SnakeHasMoved; 
    private Vector3 lastPos;
    private Vector3 tailLastPos;

    //Linked list shenanigans
    //[SerializeField] public MyLinkedList<GameObject> snakeList = new MyLinkedList<GameObject>();
    
    public LinkedList<GameObject> snakeLL = new LinkedList<GameObject>();

    private Transform snakeHeadMover;

    private Direction startDirection = Direction.Right; 
    private Direction currentDirection;
    private Direction lastDirection;
    private Vector3 continueLastMovement;

    public Vector3 LastPos => lastPos;
    
    public Vector3 TailLastPos => tailLastPos;

    public float TimeBetweenMoves
    {
        get => timeBetweenMoves;
        set => timeBetweenMoves -= value;
    }

    private enum Direction
    {
        Up,
        Down,
        Right,
        Left,
    }
    
    void Start()
    {
        LinkedListNode<GameObject> headNode = new LinkedListNode<GameObject>(this.gameObject);
        
        snakeLL.AddFirst(headNode);

        snakeHeadMover = snakeHead.transform;
        currentDirection = startDirection;
        StartCoroutine(MoveSnake());
    }

    private void Update()
    {
        HandleMovementInput();
    }

    IEnumerator MoveSnake()
    {
        while (true)
        {
            lastPos = snakeHeadMover.position;
            
            if (snakeLL.Count == 0)
            {
                LinkedListNode<GameObject> snakeHeadNode = new LinkedListNode<GameObject>(this.snakeHead);
            }

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

            LinkedListNode<GameObject> curNode = snakeLL.First;
            while (curNode != null)
            {
                if (curNode == snakeLL.First && curNode.Next != null)
                {
                    TailPiece nextPiece = curNode.Next.Value.GetComponent<TailPiece>();
                    nextPiece.LastPos = nextPiece.transform.position;
                    curNode.Next.Value.transform.position = lastPos;
                }
                else if (curNode.Next != null)
                {
                    TailPiece nextPiece = curNode.Next.Value.GetComponent<TailPiece>(); //Get ref to next piece
                    nextPiece.LastPos = nextPiece.transform.position;            //Set next pieces last pos to be it's current pos
                    nextPiece.transform.position = curNode.Value.GetComponent<TailPiece>().LastPos; // update current pos
                }
                curNode = curNode.Next;
            }

            if (snakeLL.Last.Value.TryGetComponent<TailPiece>(out TailPiece tp))
            {
                tailLastPos = tp.LastPos;
            }
            else
            {
                tailLastPos = lastPos;
            }

            SnakeHasMoved.Invoke(); //Tells our tail that we have moved
            yield return new WaitForSeconds(timeBetweenMoves);
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
