using System;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private Floor.Controller[] floors;

    public event Action<int> FloorChanged = delegate { };

    public void Initialize(Floor.Controller[] floors)
    {
        this.floors = floors;
    }
}
