using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailPieceAStar : MonoBehaviour
{
    public static Action<int> OnGameEnd;
    
    private Vector2 lastPos;
    public Vector2 LastPos
    {
        get => lastPos;
        set => lastPos = value;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        int score = other.GetComponentInParent<SnakeMovementAstar>().mySnakeLL.Count;
        OnGameEnd?.Invoke(score);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.collider.CompareTag("Player")) return;
        
        int score = other.collider.GetComponentInParent<SnakeMovementAstar>().mySnakeLL.Count;
        OnGameEnd?.Invoke(score);
    }
}
