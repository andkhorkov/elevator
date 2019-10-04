using System;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    private Floor.Controller[] floors;
    private PriorityQueue<int> pq = new PriorityQueue<int>();
    //todo: OnGoingUpRequests, OnGoingDownRequests

    public event Action<int> FloorChanged = delegate { };

    public void Initialize(Floor.Controller[] floors)
    {
        this.floors = floors;

        pq.Enqueue(1);
        pq.Enqueue(6);
        pq.Enqueue(8);
        pq.Enqueue(-3);

        while (pq.Count > 0)
        {
            Debug.Log(pq.Dequeue());
        }
    }
}
