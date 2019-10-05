using System;
using System.Collections.Generic;
using Cabin;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private float speed = 800;
    [SerializeField] private float doorCycleTime = 10;

    private Dictionary<int, Floor.FloorController> floors; // elevator might serve not all the floors, that's why it's a Dictionary
    private CabinController cabinController;
    private Transform cabin;
    private float currentDoorCycleTime;

    private State currentState;
    private State idleState;
    private State goingUpState;
    private State goingDownState;
    private State doorsCycleState;

    public event Action<int> FloorChanged = delegate { };

    public event Action EnteredIdle = delegate { };

    public event Action<int> GoalFloorReached = delegate { };
    
    public void Initialize(Dictionary<int, Floor.FloorController> floors, CabinController cabinController)
    {
        this.floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        currentFloorNum = 1;
        nextFloorNum = 2;
        goalFloorNum = 6;

        idleState = new IdleState(this);
        goingUpState = new GoingUpState(this);
        goingDownState = new GoingDownState(this);
        doorsCycleState = new DoorsCycleState(this);

        SetState(goingUpState);
        FloorChanged.Invoke(currentFloorNum);
    }

    private int currentFloorNum;
    private int goalFloorNum;
    private int nextFloorNum;

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
        GoalFloorReached.Invoke(currentFloorNum);
    }

    public void MoveCabin()
    {
        if (Mathf.Approximately(Vector3.SqrMagnitude(cabin.position - floors[nextFloorNum].Position), 0))
        {
            currentFloorNum = nextFloorNum;
            FloorChanged.Invoke(currentFloorNum);

            if (currentFloorNum == goalFloorNum)
            {
                OnReachGoalFloor();
                return;
            }

            do
            {
                ++nextFloorNum;
            }
            while (!floors.ContainsKey(nextFloorNum));
        }

        cabin.position = Vector3.MoveTowards(cabin.transform.position,
            floors[nextFloorNum].Position, speed * Time.deltaTime);
    }

    public void OpenDoors()
    {
        floors[currentFloorNum].OpenDoors();
    }

    public void CloseDoors()
    {
        floors[currentFloorNum].CloseDoors();
    }

    public void DoorsUpdate()
    {
        if (currentDoorCycleTime > doorCycleTime)
        {
            currentDoorCycleTime = 0;
            SetState(idleState); // todo: wait for open, wait for seconds, wait for close, then jump to next task
            return;
        }

        currentDoorCycleTime += Time.deltaTime;
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
