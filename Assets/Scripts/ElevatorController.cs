﻿using System;
using System.Collections.Generic;
using System.Text;
using Cabin;
using Floor;
using UnityEngine;

public enum ElevatorDirection
{
    up,
    down,
    none
}

public class ElevatorController : MonoBehaviour
{
    public struct Request : IComparable<Request>, IEqualityComparer<Request>
    {
        public ElevatorDirection DesiredDirection { get; }
        public int FloorNum { get; }

        public Request(ElevatorDirection direction, int floorNum)
        {
            DesiredDirection = direction;
            FloorNum = floorNum;
        }

        public int CompareTo(Request other)
        {
            if (DesiredDirection == ElevatorDirection.up)
            {
                return other.FloorNum < FloorNum ? 1 : -1;
            }

            if (DesiredDirection == ElevatorDirection.down)
            {
                return other.FloorNum > FloorNum ? 1 : -1;
            }

            return 0;
        }

        public bool Equals(Request x, Request y)
        {
            return x.FloorNum == y.FloorNum && x.DesiredDirection == y.DesiredDirection;
        }

        public int GetHashCode(Request req)
        {
            return req.GetHashCode();
        }
    }

    private CabinController cabinController;
    private Transform cabin;
    private float currentDoorCycleTime;
    private float speed;
    private ElevatorDirection movingDirection;

    private State currentState;
    private State idleState;
    private State movingState;
    private State doorsCycleState;

    private PriorityUQueue<Request> downRequests = new PriorityUQueue<Request>();
    private PriorityUQueue<Request> upRequests = new PriorityUQueue<Request>();
    private PriorityUQueue<Request> downDelayedRequests = new PriorityUQueue<Request>();
    private PriorityUQueue<Request> upDelayedRequests = new PriorityUQueue<Request>();
    private PriorityUQueue<Request> currentRequests;
    private PriorityUQueue<Request> currentOppositeRequests;
    private PriorityUQueue<Request> currentDelayedRequests;
    //when doors are closing we dequeuing current pq, but we don't want to dequeue the task that become first while current task is not removed yet. Some other task might want to go to the peak of the heap.
    private Queue<Request> tempBufferRequests = new Queue<Request>();  
    private Request currentRequest;
    private int nextFloorNum;

    public Dictionary<int, FloorController> Floors { get; private set; } // elevator might serve not all the floors, that's why it's a Dictionary
    public int CurrentFloorNum { get; private set; }

    public event Action<int> FloorChanged = delegate { };

    public event Action EnteredIdle = delegate { };

    public event Action<ElevatorDirection> DirectionChanged = delegate { };

    public event Action<int, ElevatorDirection> GoalFloorReached = delegate { };

    public void Initialize(Dictionary<int, FloorController> floors, CabinController cabinController, float speed)
    {
        this.speed = speed;
        Floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        CurrentFloorNum = 1;

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        doorsCycleState = new DoorsCycleState(this);

        SetState(idleState);
        FloorChanged.Invoke(CurrentFloorNum);
        GoalFloorReached.Invoke(CurrentFloorNum, ElevatorDirection.none);
        cabinController.ShowCabin(false);
    }

    private void Update()
    {
        currentState.Update();
    }

    private void SetState(State state)
    {
        if (currentState == state)
        {
            return;
        }

        if (currentState != null)
        {
            currentState.OnLeave();
        }

        currentState = state;
        currentState.OnEnter();
    }

    private void OnReachGoalFloor()
    {
        SetState(doorsCycleState);
        GoalFloorReached.Invoke(CurrentFloorNum, currentRequest.DesiredDirection);
    }

    private void JumpToNextRequest()
    {
        if(currentRequests.Count > 0)
        {
            currentRequests.Dequeue();
        }

        while (tempBufferRequests.Count > 0)
        {
            var request = tempBufferRequests.Dequeue();
            currentRequests.Enqueue(request);
        }

        if(currentRequests.Count == 0)
        {
            var dir = currentRequest.DesiredDirection;
            var floor = currentRequest.FloorNum;
            var symmetricRequest = new Request(dir == ElevatorDirection.up ? ElevatorDirection.down : ElevatorDirection.up, floor);

            if (currentOppositeRequests.Count > 0 && currentOppositeRequests.Contains(symmetricRequest))
            {
                currentOppositeRequests.Remove(symmetricRequest);
                GoalFloorReached.Invoke(CurrentFloorNum, symmetricRequest.DesiredDirection);
            }

            if (currentOppositeRequests.Count > 0)
            {
                currentRequests = currentOppositeRequests;
            }
            else if (currentDelayedRequests.Count > 0)
            {
                currentRequests = currentDelayedRequests;
            }

            if (currentRequests.Count == 0)
            {
                SetState(idleState);
                return;
            }
        }

        currentRequest = currentRequests.Peek;

        SetState(movingState);
    }

    public void AddRequest(int desiredFloorNum, ElevatorDirection desiredDirection)
    {
        if (desiredDirection == ElevatorDirection.none) // cabin btn
        {
            desiredDirection = GetDirectionToRequestedFloor(desiredFloorNum);
        }

        var request = new Request(desiredDirection, desiredFloorNum);

        if (currentState == idleState)
        {
            if (request.DesiredDirection == ElevatorDirection.up)
            {
                currentRequests = upRequests;
                currentOppositeRequests = downRequests;
                currentDelayedRequests = upDelayedRequests;
            }
            else
            {
                currentRequests = downRequests;
                currentOppositeRequests = upRequests;
                currentDelayedRequests = downDelayedRequests;
            }

            currentRequests.Enqueue(request);
            currentRequest = currentRequests.Peek;
            SetState(movingState);

            Debug.Log(0);
        }
        else if (currentState == doorsCycleState)
        {
            tempBufferRequests.Enqueue(request);

            Debug.Log(6);
            return;
        }
        else if (request.Equals(currentRequest))
        {
            GoalFloorReached.Invoke(CurrentFloorNum, currentRequest.DesiredDirection);
            Debug.Log(1);
            return;
        }
        else if (request.DesiredDirection != currentRequest.DesiredDirection)
        {
            currentOppositeRequests.Enqueue(request);
            Debug.Log(2);
        }
        else if (movingDirection != desiredDirection)
        {
            currentRequests.Enqueue(request);
            Debug.Log(3);
        }
        else if ((desiredFloorNum - CurrentFloorNum) * (movingDirection == ElevatorDirection.up ? 1 : -1) > 0)
        {
            currentRequests.Enqueue(request);
            Debug.Log(4);
        }
        else if (currentOppositeRequests.Count > 0)
        {
            currentDelayedRequests.Enqueue(request);
            Debug.Log(5);
        }
        else
        {
            Debug.Log($"CurrentFloorNum: {CurrentFloorNum}, movingDirection: {movingDirection}, requestedFloor: {request.FloorNum}, requestedDir: {request.DesiredDirection}, currentRequest: {currentRequest.FloorNum}-{currentRequest.DesiredDirection}");
            currentRequests.Enqueue(request);
        }

        currentRequest = currentRequests.Peek;
    }

    private void OnStartMoving()
    {
        movingDirection = GetDirectionToRequestedFloor(currentRequest.FloorNum);
        DirectionChanged.Invoke(movingDirection);
        nextFloorNum = movingDirection == ElevatorDirection.up ? CurrentFloorNum + 1 : CurrentFloorNum - 1;
    }

    private ElevatorDirection GetDirectionToRequestedFloor(int floorNum)
    {
        return floorNum > CurrentFloorNum ? ElevatorDirection.up : ElevatorDirection.down;
    }

    public void MoveCabin()
    {
        if (CurrentFloorNum == currentRequest.FloorNum && cabin.position == Floors[CurrentFloorNum].Position)
        {
            OnReachGoalFloor();
            return;
        }

        if (cabin.position == Floors[nextFloorNum].Position)
        {
            CurrentFloorNum = nextFloorNum;
            FloorChanged.Invoke(CurrentFloorNum);

            do
            {
                if (movingDirection == ElevatorDirection.up)
                {
                    ++nextFloorNum;
                    
                }
                else
                {
                    --nextFloorNum;
                }

                if (nextFloorNum <= 0 || nextFloorNum > Floors.Count)
                {
                    return;
                }
            }
            while (!Floors.ContainsKey(nextFloorNum));

            return;
        }

        cabin.position = Vector3.MoveTowards(cabin.transform.position,
            Floors[nextFloorNum].Position, speed * Time.deltaTime);
    }

    public void OpenDoors()
    {
        Floors[CurrentFloorNum].OpenDoors();
        cabinController.ShowCabin(true);
    }

    public void CloseDoors()
    {
        Floors[CurrentFloorNum].CloseDoors();
    }

    public void OnDoorsClosed()
    {
        JumpToNextRequest(); // jump to next task
        cabinController.ShowCabin(false);
    }

    public void DoorsUpdate()
    {
        Floors[CurrentFloorNum].DoorsUpdate();
    }

    public void OnEnteredIdle()
    {
        EnteredIdle.Invoke();
    }

    public abstract class State
    {
        protected ElevatorController elevator;

        protected State(ElevatorController elevator)
        {
            this.elevator = elevator;
        }

        public abstract void Update();
        public abstract void OnEnter();
        public abstract void OnLeave();
    }

    public class MovingState : State
    {
        public MovingState(ElevatorController elevator) : base(elevator)
        {
        }

        public override void Update()
        {
            elevator.MoveCabin();
        }

        public override void OnEnter()
        {
            elevator.OnStartMoving();
        }

        public override void OnLeave()
        {
        }
    }

    public class IdleState : State
    {
        public IdleState(ElevatorController elevator) : base(elevator)
        {
        }

        public override void Update()
        {
        }

        public override void OnEnter()
        {
            elevator.OnEnteredIdle();
        }

        public override void OnLeave()
        {
        }
    }

    public class DoorsCycleState : State
    {
        public DoorsCycleState(ElevatorController elevator) : base(elevator)
        {
        }

        public override void Update()
        {
            elevator.DoorsUpdate();
        }

        public override void OnEnter()
        {
            elevator.OpenDoors();
        }

        public override void OnLeave()
        {
            elevator.CloseDoors();
        }
    }
}
