using System;
using System.Collections.Generic;
using System.Linq;
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
    public struct Request
    {
        public ElevatorDirection DesiredDirection { get; private set; }
        public int FloorNum { get; private set; }

        public Request(ElevatorDirection direction, int floorNum)
        {
            DesiredDirection = direction;
            FloorNum = floorNum;
        }
    }

    [SerializeField] private float speed = 800;
    [SerializeField] private float doorCycleTime = 4;

    private Dictionary<int, FloorController> floors; // elevator might serve not all the floors, that's why it's a Dictionary
    private CabinController cabinController;
    private Transform cabin;
    private float currentDoorCycleTime;
    private ElevatorDirection movingDirection;

    private State currentState;
    private State idleState;
    private State movingState;
    private State goingUpState;
    private State goingDownState;
    private State doorsCycleState;

    private LinkedList<Request> currentDirectionRequests = new LinkedList<Request>();
    private LinkedList<Request> oppositeRequests = new LinkedList<Request>();
    private LinkedList<Request> delayedRequests = new LinkedList<Request>();
    private LinkedList<Request> currentRequests;
    private Request currentRequest;

    private int goalFloorNum;
    private int nextFloorNum;

    public int CurrentFloorNum { get; private set; }

    public event Action<int> FloorChanged = delegate { };

    public event Action EnteredIdle = delegate { };

    public event Action<ElevatorDirection> DirectionChanged = delegate { }; 
    
    public void Initialize(Dictionary<int, FloorController> floors, CabinController cabinController)
    {
        this.floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        CurrentFloorNum = 1;

        idleState = new IdleState(this);
        movingState = new MovingState(this);
        goingUpState = new GoingUpState(this);
        goingDownState = new GoingDownState(this);
        doorsCycleState = new DoorsCycleState(this);

        SetState(idleState);
        FloorChanged.Invoke(CurrentFloorNum);
        floors[CurrentFloorNum].OnGoalFloorReached(CurrentFloorNum, ElevatorDirection.none);
    }

    private void Update()
    {
        currentState.Update();
    }

    private void SetState(State state)
    {
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
        floors[CurrentFloorNum].OnGoalFloorReached(CurrentFloorNum, currentDirectionRequests.First.Value.DesiredDirection);
    }

    private void JumpToNextRequest()
    {
        var node = currentDirectionRequests.First.Next;

        if (node == null)
        {
            if (oppositeRequests.Count == 0)
            {
                SetState(idleState);
                return;
            }

            currentDirectionRequests = oppositeRequests;
            oppositeRequests = delayedRequests;
            currentRequest = currentDirectionRequests.First.Value;
            PrintRequests();
            SetState(movingState);
            return;
        }

        currentRequest = node.Value;
        currentDirectionRequests.RemoveFirst();
        SetState(movingState);
        PrintRequests();
    }

    public void AddRequest(int floorNum, ElevatorDirection desiredDirection)
    {
        var request = new Request(desiredDirection, floorNum);

        if (currentState == idleState)
        {
            currentRequest = request;
            currentDirectionRequests.AddFirst(request);
            SetState(movingState);
            PrintRequests();
            return;
        }

        if (movingDirection != desiredDirection)
        {
            if (floorNum > CurrentFloorNum)
            {
                var node = currentDirectionRequests.First;

                while (node != null)
                {
                    if (node.Value.FloorNum < floorNum)
                    {
                        currentDirectionRequests.AddBefore(node, request);
                        currentRequest = request;
                        goalFloorNum = request.FloorNum;
                        PrintRequests();
                        return;
                    }

                    node = node.Next;
                }

                currentDirectionRequests.AddLast(request);
                PrintRequests();
                return;
            }
        }
    }

    private void PrintRequests()
    {
        var root = currentDirectionRequests.First;
        var sb = new StringBuilder();

        while (root != null)
        {
            var request = root.Value;
            sb.Append($"{request.FloorNum}:{request.DesiredDirection},  ");

            root = root.Next;
        }

        Debug.Log(sb.ToString());
    }

    private void OnStartMoving()
    {
        var request = currentDirectionRequests.First.Value;
        movingDirection = GetDirectionToRequestedFloor(request.FloorNum, CurrentFloorNum);
        DirectionChanged.Invoke(movingDirection);
        goalFloorNum = request.FloorNum;
        nextFloorNum = movingDirection == ElevatorDirection.up ? CurrentFloorNum + 1 : CurrentFloorNum - 1;
    }

    private static ElevatorDirection GetDirectionToRequestedFloor(int floorNum, int currentFloorNum)
    {
        return floorNum > currentFloorNum ? ElevatorDirection.up : ElevatorDirection.down;
    }

    public void MoveCabin()
    {
        if (CurrentFloorNum == goalFloorNum)
        {
            OnReachGoalFloor();
            return;
        }

        if (Mathf.Approximately(Vector3.SqrMagnitude(cabin.position - floors[nextFloorNum].Position), 0))
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

                if (nextFloorNum <= 0 || nextFloorNum > floors.Count)
                {
                    return;
                }
            }
            while (!floors.ContainsKey(nextFloorNum));
        }

        cabin.position = Vector3.MoveTowards(cabin.transform.position,
            floors[nextFloorNum].Position, speed * Time.deltaTime);
    }

    public void OpenDoors()
    {
        floors[CurrentFloorNum].OpenDoors();
    }

    public void CloseDoors()
    {
        floors[CurrentFloorNum].CloseDoors();
    }

    public void OnDoorsClosed()
    {
        Debug.Log("closed");
        JumpToNextRequest(); // jump to next task
    }

    public void DoorsUpdate()
    {
        floors[CurrentFloorNum].DoorsUpdate();
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

    public class GoingUpState : State
    {
        public GoingUpState(ElevatorController elevator) : base(elevator)
        {
        }

        public override void Update()
        {
            elevator.MoveCabin();
        }

        public override void OnEnter()
        {
        }

        public override void OnLeave()
        {
        }
    }

    public class GoingDownState : State
    {
        public GoingDownState(ElevatorController elevator) : base(elevator)
        {
        }

        public override void Update()
        {
            elevator.MoveCabin();
        }

        public override void OnEnter()
        {
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
