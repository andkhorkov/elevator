using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class FloorBtn
{
    [SerializeField] private Collider2D col;

    private Floor floor;

    public void Initialize(Floor floor)
    {
        this.floor = floor;
        Debug.Log(floor.Id);
    }

    public void Update()
    {
        Vector3 clickPos = Vector3.zero;
        bool isClicked = false;

        if (Input.GetMouseButtonDown(0))
        {
            clickPos = Input.mousePosition;
            isClicked = true;
        }
        else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            clickPos = Input.GetTouch(0).position;
            isClicked = true;
        }

        if (isClicked)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(clickPos);
            var worldClickPos = new Vector2(worldPos.x, worldPos.y);

            if (col == Physics2D.OverlapPoint(worldClickPos))
            {
                Debug.Log(floor.Id);
            }
        }
    }
}
