using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridGenerator : MonoBehaviour
{
    [Header("Tiles that can spawn"), Tooltip("Currently index 0 = wall, index 1 = floor.")]
    [SerializeField] private List<GameObject> tileList;

    [Header("Food to spawn")] [SerializeField]
    private GameObject foodPrefab;
    private Vector3 currentFoodPos;

    private int foodEaten = 0;
    private Camera cam;
    private float height, width;
    private List<GameObject> tilesToSpawnFoodOn = new List<GameObject>();

    private void Awake()
    {
        Time.timeScale = 1f; 
        cam = Camera.main;

        height = Mathf.Floor(2f * cam.orthographicSize);
        width = Mathf.Ceil(height * cam.aspect);

        height /= 2;
        width /= 2;

        FoodEaten.OnFoodEaten += HandleFoodEaten;
    }

    private void OnDestroy()
    {
        FoodEaten.OnFoodEaten -= HandleFoodEaten;
    }

    void Start()
    {
        GeneratePlayableGridArea();
        SpawnNewFood();
    }

    private void HandleFoodEaten()
    {
        SpawnNewFood();
        foodEaten++;

        if (foodEaten < 5) return;
        
        CreateRandomWalls(2);
        foodEaten = 0;
    }

    public void SpawnNewFood()
    {
        int getRandomTile = Random.Range(0, tilesToSpawnFoodOn.Count);
        GameObject food = Instantiate(foodPrefab);
        food.transform.position = tilesToSpawnFoodOn[getRandomTile].transform.position;
    }
    
    public void CreateRandomWalls(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int getRandomTile = Random.Range(0, tilesToSpawnFoodOn.Count);
            GameObject wall = Instantiate(tileList[0]);
            wall.transform.position = tilesToSpawnFoodOn[getRandomTile].transform.position;
            tilesToSpawnFoodOn.RemoveAt(getRandomTile);
        }
    }
    
    private void GeneratePlayableGridArea()
    {
        GameObject tile;
        
        int lowestX = (int) (width * -1);
        int lowestY = (int) (height * -1);

        for (int y = lowestY; y <= height; y++)
        {
            for (int x = lowestX; x <= width; x++)
            {
                if (x == lowestX || x == (int)width || y == lowestY || y == (int)height)
                {
                    tile = Instantiate(tileList[0], new Vector3(x, y,0), quaternion.identity);
                    tile.transform.parent = transform;
                }
                else
                {
                    tile = Instantiate(tileList[1], new Vector3(x, y,0), quaternion.identity);
                    tile.transform.parent = transform;
                    
                    tilesToSpawnFoodOn.Add(tile);
                }

            }
        }
    }
}
