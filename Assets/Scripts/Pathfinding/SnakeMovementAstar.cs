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

    [SerializeField] private Grid grid;

    //Used for tail to follow snake head (Hidden because we progrematically access these)
    [HideInInspector] public UnityEvent SnakeHasMoved; 
    
    public static event Action OnGameOver;
    
    private Vector3 lastPos;
    private Vector3 tailLastPos;

    private Transform snakeHeadMover;

    private Direction startDirection = Direction.Right; 
    private Direction currentDirection;
    private Direction lastDirection;
    private Vector3 continueLastMovement;

    [SerializeField]private List<Vector2> pathToFollow = new List<Vector2>();
    private List<Vector2> allTailPositions;

    //Getters
    public MyLinkedList<GameObject> mySnakeLL = new MyLinkedList<GameObject>();
    public List<Vector2> AllTailPositions => allTailPositions;
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
            lastPos = snakeHeadMover.position; //Save a reference to our position before we move. Used to update tail.
            
            if (pathToFollow.Count > 0)
            {
                snakeHeadMover.position = pathToFollow[0]; //Move snake head to first "waypoint"
                pathToFollow.RemoveAt(0); //remove current waypoint after moving to it

                UpdateTailPositions(); //Update all the tail positions using our linked list implementation
                SnakeHasMoved.Invoke(); //Tells our tail that we have moved
                grid.RecalculateGrid();
            }
            else
            {
                //Try to move right
                bool hasMoved = false;
                Vector2 posTocheck = (Vector2)snakeHeadMover.transform.position + Vector2.right;
                if (grid.CheckIfWalkable(posTocheck) && hasMoved == false)
                {
                    hasMoved = MoveToNewPosAndSeeIfWeCanFindAPath(posTocheck);
                }
                //Try to move left
                posTocheck = (Vector2)snakeHeadMover.transform.position + Vector2.left;
                if (grid.CheckIfWalkable(posTocheck) && hasMoved == false)
                {
                    hasMoved = MoveToNewPosAndSeeIfWeCanFindAPath(posTocheck);
                }
                //Try to move up
                posTocheck = (Vector2)snakeHeadMover.transform.position + Vector2.up;
                if (grid.CheckIfWalkable(posTocheck) && hasMoved == false)
                {
                    hasMoved = MoveToNewPosAndSeeIfWeCanFindAPath(posTocheck);
                }
                //Try to move down
                posTocheck = (Vector2)snakeHeadMover.transform.position + Vector2.down;
                if (grid.CheckIfWalkable(posTocheck) && hasMoved == false)
                {
                    hasMoved = MoveToNewPosAndSeeIfWeCanFindAPath(posTocheck);
                }

                if (!hasMoved)
                {
                    OnGameOver?.Invoke();
                }
            }
            
            yield return new WaitForSeconds(timeBetweenMoves);
        }
    }

    private bool MoveToNewPosAndSeeIfWeCanFindAPath(Vector2 posTocheck)
    {
        bool hasMoved;
        snakeHeadMover.position = posTocheck;
        UpdateTailPositions();
        SnakeHasMoved.Invoke();
        hasMoved = true;
        grid.RecalculateGrid();
        return hasMoved;
    }

    private void UpdateTailPositions()
    {
        MyListNode<GameObject> curNode = mySnakeLL.First();

        while (curNode != null)
        {
            if (curNode == mySnakeLL.First() && curNode.next != null)
            {
                TailPieceAStar nextPiece = curNode.next.data.GetComponent<TailPieceAStar>();
                nextPiece.LastPos = nextPiece.transform.position;
                curNode.next.data.transform.position = lastPos;
            }
            else if (curNode.next != null)
            {
                TailPieceAStar nextPiece = curNode.next.data.GetComponent<TailPieceAStar>(); //Get ref to next piece
                nextPiece.LastPos = nextPiece.transform.position; //Set next pieces last pos to be it's current pos
                nextPiece.transform.position = curNode.data.GetComponent<TailPieceAStar>().LastPos; // Update the tail nodes current pos
            }
            curNode = curNode.next;
        }
        
        //Keep a reference to where we should add the next tail piece
        if (mySnakeLL.Last().data.TryGetComponent<TailPieceAStar>(out TailPieceAStar tp))
        {
            tailLastPos = tp.LastPos;
        }
        else
        {
            tailLastPos = lastPos;
        }
        
        //Get a list of all the positions of the tail so we can set these as not-walkable
        curNode = mySnakeLL.First();
        allTailPositions = new List<Vector2>();
        while (curNode.next != null)
        {
            TailPieceAStar curTail = curNode.next.data.GetComponent<TailPieceAStar>();
            allTailPositions.Add(curTail.transform.position);
            curNode = curNode.next;
        }
    }
}
