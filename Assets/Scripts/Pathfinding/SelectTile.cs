using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectTile : MonoBehaviour
{
    private Camera mainCam;
    [SerializeField] private GameObject firstSelectedTile;
    [SerializeField] private GameObject secondSelectedTile;

    [SerializeField] private Vector2 pos1, pos2;
    
    private Color floorColor = new Color(1f, 0.45f, 0.85f);
    

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Pressed m1");

            RaycastHit2D hit = Physics2D.Raycast(mainCam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider == null) return;

            Debug.Log(hit.collider.name);
            hit.collider.TryGetComponent<SpriteRenderer>(out SpriteRenderer hitRenderer);

            if (firstSelectedTile != null && secondSelectedTile != null)
            {
                firstSelectedTile.gameObject.GetComponent<SpriteRenderer>().color = floorColor;
                secondSelectedTile.gameObject.GetComponent<SpriteRenderer>().color = floorColor;
                
                firstSelectedTile = null;
                secondSelectedTile = null;
            }
            
            if (firstSelectedTile == null)
            {
                firstSelectedTile = hit.collider.gameObject;
                pos1 = firstSelectedTile.transform.position;
                
                hitRenderer.color = Color.blue;
            }
            else
            {
                secondSelectedTile = hit.collider.gameObject;
                pos2 = secondSelectedTile.transform.position;
                
                hitRenderer.color = Color.yellow;
            }
        }
    }
}
