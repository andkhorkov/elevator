using System;
using System.Collections.Generic;
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
        public ElevatorDirection Direction { get; }
        public int FloorNum { get; }

        public Request(ElevatorDirection direction, int floorNum)
        {
            Direction = direction;
            FloorNum = floorNum;
        }

        public int CompareTo(Request other)
        {
            if (Direction == ElevatorDirection.up)
            {
                return other.FloorNum < FloorNum ? 1 : -1;
            }

            if (Direction == ElevatorDirection.down)
            {
                return other.FloorNum > FloorNum ? 1 : -1;
            }

            return 0;
        }

        public bool Equals(Request x, Request y)
        {
            return x.FloorNum == y.FloorNum && x.Direction == y.Direction;
        }

        public int GetHashCode(Request req)
        {
            return req.GetHashCode();
        }

        public override string ToString()
        {
            return $"{FloorNum} : {Direction}";
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
    private PriorityUQueue<Request> currRequests;
    private PriorityUQueue<Request> currOppositeRequests;
    private PriorityUQueue<Request> currDelayedRequests;

    private Request currRequest;
    private int currFloorNum;
    private int nextFloorNum;

    public bool IsIdle => currentState == idleState;
    public int CurrFloorNum => currFloorNum;
    public Dictionary<int, FloorController> Floors { get; private set; } // elevator might serve not all the floors, that's why it's a Dictionary. Maybe some DummyFloor class also needed.

    public event Action<int> FloorChanged = delegate { };

    public event Action EnteredIdle = delegate { };

    public event Action<ElevatorDirection> DirectionChanged = delegate { };

    public event Action<Request> GoalFloorReached = delegate { };

    public event Action<Request> RequestNoLongerActual = delegate { };

    public void Initialize(Dictionary<int, FloorController> floors, CabinController cabinController, float speed)
    {
        this.speed = speed;
        Floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        currFloorNum = 1;

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        doorsCycleState = new DoorsCycleState(this);
        SetState(idleState);

        FloorChanged.Invoke(currFloorNum);
        GoalFloorReached.Invoke(new Request(ElevatorDirection.none, currFloorNum));
        cabinController.ShowCabin(false);
    }

    //todo: make it not that scary
    public void AddRequest(int desiredFloorNum, ElevatorDirection desiredDirection)
    {
        if (desiredDirection == ElevatorDirection.none) // from cabin btn
        {
            desiredDirection = GetDirectionToRequestedFloor(desiredFloorNum);
        }

        var request = new Request(desiredDirection, desiredFloorNum);

        if (IsIdle)
        {
            if (request.Direction == ElevatorDirection.up)
            {
                currRequests = upRequests;
                currOppositeRequests = downRequests;
                currDelayedRequests = upDelayedRequests;
            }
            else
            {
                currRequests = downRequests;
                currOppositeRequests = upRequests;
                currDelayedRequests = downDelayedRequests;
            }

            currRequests.Enqueue(request);
            currRequest = request;
            SetState(movingState);
            PrintElevatorState();
            return;
        }

        if (request.Equals(currRequest))
        {
            RequestNoLongerActual.Invoke(currRequest);
        }
        else if (request.Direction != currRequest.Direction)
        {
            currOppositeRequests.Enqueue(request);
        }
        else if (movingDirection != request.Direction)
        {
            currRequests.Enqueue(request);
        }
        else if ((movingDirection == ElevatorDirection.up ? 1 : -1) * (request.FloorNum - currFloorNum) > 0)
        {
            currRequests.Enqueue(request);
        }
        else
        {
            currDelayedRequests.Enqueue(request);
        }

        if (IsIdle)
        {
            return;
        }

        if (currentState != doorsCycleState)
        {
            currRequest = currRequests.Peek;

            PrintElevatorState();
        }
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
        currRequests.Dequeue();
        SetState(doorsCycleState);
        GoalFloorReached.Invoke(currRequest);
        PrintElevatorState();
    }

    private void PrintElevatorState()
    {
        Debug.Log($"floor: {currFloorNum}, current: {currRequests.Print()} || opposite: {currOppositeRequests.Print()} || delayed: {currDelayedRequests.Print()}");
    }

    private void JumpToNextRequest()
    {
        if (currRequests.Count == 0)
        {
            if (currOppositeRequests.Count > 0)
            {
                currRequests = currOppositeRequests;
            }
            else if (currDelayedRequests.Count > 0)
            {
                currRequests = currDelayedRequests;
            }
            else
            {
                SetState(idleState);
                return;
            }
        }

        currRequest = currRequests.Peek;

        if (currRequest.FloorNum == currFloorNum)
        {
            RequestNoLongerActual(currRequest);
            currRequests.Dequeue();
            JumpToNextRequest();

            if (currRequests.Count == 0)
            {
                SetState(idleState);
                return;
            }
        }

        SetState(movingState);
    }

    private void OnStartMoving()
    {
        movingDirection = GetDirectionToRequestedFloor(currRequest.FloorNum);
        DirectionChanged.Invoke(movingDirection);
        nextFloorNum = movingDirection == ElevatorDirection.up ? currFloorNum + 1 : currFloorNum - 1;
        nextFloorNum = Mathf.Clamp(nextFloorNum, 1, Floors.Count);
    }

    private ElevatorDirection GetDirectionToRequestedFloor(int floorNum)
    {
        return floorNum > currFloorNum ? ElevatorDirection.up : ElevatorDirection.down;
    }

    public void MoveCabin()
    {
        if (currFloorNum == currRequest.FloorNum && cabin.position == Floors[currFloorNum].Position)
        {
            OnReachGoalFloor();
            return;
        }

        if (cabin.position == Floors[nextFloorNum].Position)
        {
            currFloorNum = nextFloorNum;
            FloorChanged.Invoke(currFloorNum);

            do
            {
                nextFloorNum = movingDirection == ElevatorDirection.up ? nextFloorNum + 1 : nextFloorNum - 1;
                nextFloorNum = Mathf.Clamp(nextFloorNum, 1, Floors.Count);

                if (nextFloorNum <= 0 || nextFloorNum > Floors.Count)
                {
                    return;
                }

            } while (Floors[nextFloorNum] == null);  

            return;
        }

        cabin.position = Vector3.MoveTowards(cabin.transform.position,
            Floors[nextFloorNum].Position, speed * Time.deltaTime);
    }

    public void OpenDoors()
    {
        if (!Floors.TryGetValue(currFloorNum, out var floor) || floor.Num != currFloorNum)
        {
            Debug.LogWarning("either floor number is not or elevator not serves this floor");
            return;
        }

        floor.OpenDoors();
        cabinController.ShowCabin(true);
    }

    public void CloseDoors()
    {
        Floors[currFloorNum].CloseDoors();
    }

    public void OnDoorsClosed()
    {
        JumpToNextRequest(); 
        cabinController.ShowCabin(false);
    }

    public void DoorsUpdate()
    {
        Floors[currFloorNum].DoorsUpdate();
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
