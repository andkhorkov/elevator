namespace Elevator
{
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
