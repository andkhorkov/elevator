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

    public FloorBtn(Floor floor)
    {
        this.floor = floor;
        Debug.Log(floor.Id);
    }

    private void OnMouseDown()
    {
        Debug.Log("suka");
    }
}
