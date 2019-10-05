using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private float speed = 800;

    private Dictionary<int, Floor.Controller> floors; // elevator might serve not all the floors, that's why it's a Dictionary
    private Cabin.Controller cabinController;
    private Transform cabin;
    private State currentState;
    private State idleState;
    private State goingUpState;
    private State goingDownState;

    public event Action<int> FloorChanged = delegate { };
    
    public void Initialize(Dictionary<int, Floor.Controller> floors, Cabin.Controller cabinController)
    {
        this.floors = floors;
        this.cabinController = cabinController;
        cabin = cabinController.transform;
        currentFloorNum = 1;
        nextFloorNum = 2;
        goalFloorNum = 5;

        idleState = new IdleState(this);
        goingUpState = new GoingUpState(this);
        goingDownState = new GoingDownState(this);
        SetState(goingUpState);
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

    public void MoveCabin()
    {
        if (Mathf.Approximately(Vector3.SqrMagnitude(cabin.position - floors[nextFloorNum].Position), 0))
        {
            currentFloorNum = nextFloorNum;
            FloorChanged.Invoke(currentFloorNum);

            do
            {
                ++nextFloorNum;
            }
            while (!floors.ContainsKey(nextFloorNum));
        }

        if (currentFloorNum == goalFloorNum)
        {
            SetState(idleState); // jump to next task
            return;
        }

        cabin.position = Vector3.MoveTowards(cabin.transform.position,
            floors[nextFloorNum].Position, speed * Time.deltaTime);
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
        }

        public override void OnLeave()
        {
        }
    }
}
