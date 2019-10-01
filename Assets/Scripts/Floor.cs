using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftDoor;
    [SerializeField] private SpriteRenderer rightDoor;

    public Vector2 Size => leftDoor.sprite.bounds.size;
}
