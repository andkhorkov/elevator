namespace Elevator
{
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
}
