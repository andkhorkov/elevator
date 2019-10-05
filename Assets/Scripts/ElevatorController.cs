﻿using System;
using System.Collections.Generic;
using Cabin;
using Floor;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private float speed = 800;
    [SerializeField] private float doorCycleTime = 10;

    private Dictionary<int, FloorController> floors; // elevator might serve not all the floors, that's why it's a Dictionary
    private CabinController cabinController;
    private Transform cabin;
    private float currentDoorCycleTime;

    private State currentState;
    private State idleState;
    private State goingUpState;
    private State goingDownState;
    private State doorsCycleState;

    private int goalFloorNum;
    private int nextFloorNum;

    public int CurrentFloorNum { get; private set; }

    public event Action<int> FloorChanged = delegate { };

    public event Action EnteredIdle = delegate { };

    public event Action<int> GoalFloorReached = delegate { };
    
    public void Initialize(Dictionary<int, FloorController> floors, CabinController cabinController)
    {
        this.floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        CurrentFloorNum = 1;

        idleState = new IdleState(this);
        goingUpState = new GoingUpState(this);
        goingDownState = new GoingDownState(this);
        doorsCycleState = new DoorsCycleState(this);

        SetState(idleState);
        FloorChanged.Invoke(CurrentFloorNum);
        GoalFloorReached.Invoke(CurrentFloorNum);
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
        GoalFloorReached.Invoke(CurrentFloorNum);
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
                ++nextFloorNum;
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
        SetState(idleState); // jump to next task
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
