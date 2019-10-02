using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private Floor[] floors;

    public void Initialize(Floor[] floors)
    {
        this.floors = floors;
    }
}
