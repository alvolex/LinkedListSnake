using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollision : MonoBehaviour
{
    public static Action<int> OnGameOver; 
    
    private float lowestX, highestX;
    private float lowestY, highestY;

    private Camera cam;
    private float height, width;
    
    private void Start()
    {
        CalculateMapBounds();
    }

    private void CalculateMapBounds()
    {
        cam = Camera.main;
        height = Mathf.Floor(2f * cam.orthographicSize);
        width = Mathf.Ceil(height * cam.aspect);
        height /= 2;
        width /= 2;

        lowestX = (int) width * -1;
        highestX = (int) width;

        lowestY = (int) height * -1;
        highestY = (int) height;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (transform.position.x == lowestX)
            {
                other.transform.position = new Vector3(highestX - 1, other.transform.position.y);
            }
            else if (transform.position.x == highestX)
            {
                other.transform.position = new Vector3(lowestX + 1, other.transform.position.y);
            }else if (transform.position.y == lowestY)
            {
                other.transform.position = new Vector3(other.transform.position.x, highestY - 1);
            }
            else if (transform.position.y == highestY)
            {
                other.transform.position = new Vector3(other.transform.position.x, lowestY + 1);
            }
            else
            {
                Time.timeScale = 0f;
                int score = other.GetComponentInParent<SnakeMovement>().mySnakeLL.Count;
                OnGameOver?.Invoke(score);
            }

        }
    }
}
