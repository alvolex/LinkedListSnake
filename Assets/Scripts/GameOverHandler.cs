using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Canvas gameOverCanvas;
    
    private void Awake()
    {
        TailPiece.OnGameEnd += HandleGameOver;
        WallCollision.OnGameOver += HandleGameOver;
        SnakeMovementAstar.OnGameOver += AStarGameOver;
    }
    
    private void OnDestroy()
    {
        TailPiece.OnGameEnd -= HandleGameOver;
        WallCollision.OnGameOver -= HandleGameOver;
        SnakeMovementAstar.OnGameOver -= AStarGameOver;
    }
    
    private void AStarGameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        scoreText.text = $"No path available to food :(";
    }

    private void HandleGameOver(int score)
    {
        gameOverCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f;
        scoreText.text = $"Score: {score - 1}";
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
    
}
