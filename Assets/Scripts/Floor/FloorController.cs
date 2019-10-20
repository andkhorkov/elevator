using Elevator;
using UnityEngine;

namespace Floor
{
    public class FloorController : ElevatorElement
    {
        [SerializeField] private DoorController doorController;
        [SerializeField] private FloorDisplay floorDisplay;
        [SerializeField] private FloorBtn btnUp;
        [SerializeField] private FloorBtn btnDown;

        private BasementController basement;
        private ElevatorController elevator;

        public FloorBtn BtnUp => btnUp;
        public FloorBtn BtnDown => btnDown;

        public DoorController DoorController => doorController;

        public int Num { get; private set; }

        public Vector3 Position { get; private set; }

        public void Initialize(int num, ElevatorController elevator, BasementController basement)
        {
            Num = num;
            Position = transform.position;
            this.elevator = elevator;
            this.basement = basement;

            elevator.FloorChanged += OnFloorChanged;
            elevator.EnteredIdle += OnEnteredIdle;
            elevator.DirectionChanged += OnDirectionChanged;

            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;
        }

        protected override void Unsubscribes()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.EnteredIdle -= OnEnteredIdle;
            elevator.DirectionChanged -= OnDirectionChanged;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual -= OnRequestNoLongerActual;
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            btnUp.SetDefaultColor();
            btnDown.SetDefaultColor();
            SetActiveUpBtn(true);
            SetActiveDownBtn(true);
            floorDisplay.Reset();
        }

        public void OnDirectionChanged(ElevatorDirection direction)
        {
            floorDisplay.OnDirectionChanged(direction);
        }

        public void OpenDoors()
        {
            doorController.Open();
        }

        public void CloseDoors()
        {
            doorController.Close();
        }

        public void DoorsUpdate()
        {
            doorController.DoorsUpdate();
        }

        public void OnDoorsClosed()
        {
            elevator.OnDoorsClosed();
        }

        public void SetActiveUpBtn(bool active)
        {
            btnUp.gameObject.SetActive(active);
        }

        public void SetActiveDownBtn(bool active)
        {
            btnDown.gameObject.SetActive(active);
        }

        public void OnButtonClicked(ElevatorDirection direction)
        {
            basement.AddRequest(Num, direction, elevator);
        }

        private void OnFloorChanged(int floorNum)
        {
            floorDisplay.OnFloorChanged(floorNum);
        }

        private void OnEnteredIdle()
        {
            floorDisplay.OnEnteredIdle();
        }

        private void OnGoalFloorReached(ElevatorController.Request request, ElevatorController elevator)
        {
            SetBtnsState(request);
        }

        private void OnRequestNoLongerActual(ElevatorController.Request request, ElevatorController elevator)
        {
            SetBtnsState(request);
        }

        private void SetBtnsState(ElevatorController.Request request)
        {
            if (request.FloorNum != Num)
            {
                return;
            }

            if (request.Direction == ElevatorDirection.up)
            {
                btnUp.SetDefaultColor();
            }

            if (request.Direction == ElevatorDirection.down)
            {
                btnDown.SetDefaultColor();
            }
        }

        public void OnGoalFloorReached()
        {
            floorDisplay.OnGoalFloorReached();
        }
    }
}