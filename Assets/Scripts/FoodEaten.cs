using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodEaten : MonoBehaviour
{
    public static Action OnFoodEaten;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnFoodEaten?.Invoke();
            Destroy(gameObject);
        }
    }
}
