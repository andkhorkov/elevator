using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private FloorController[] floors;

    public void Initialize(FloorController[] floors)
    {
        this.floors = floors;
    }
}
