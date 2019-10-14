using UnityEngine;

namespace Floor
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private DoorController doorController;
        [SerializeField] private FloorDisplay floorDisplay;
        [SerializeField] private FloorBtn btnUp;
        [SerializeField] private FloorBtn btnDown;

        private ElevatorController elevator;

        public FloorBtn BtnUp => btnUp;
        public FloorBtn BtnDown => btnDown;

        public DoorController DoorController => doorController;

        public int Num { get; private set; }

        public Vector3 Position { get; private set; }

        public void Initialize(int num, ElevatorController elevator)
        {
            Num = num;
            Position = transform.position;
            this.elevator = elevator;

            elevator.FloorChanged += OnFloorChanged;
            elevator.EnteredIdle += OnEnteredIdle;
            elevator.DirectionChanged += OnDirectionChanged;
            elevator.GoalFloorReached += OnGoalFloorReached;
            elevator.RequestNoLongerActual += OnRequestNoLongerActual;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.EnteredIdle -= OnEnteredIdle;
            elevator.DirectionChanged -= OnDirectionChanged;
            elevator.GoalFloorReached -= OnGoalFloorReached;
            elevator.RequestNoLongerActual -= OnRequestNoLongerActual;
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

        public void SwitchOffDownBtn()
        {
            btnDown.gameObject.SetActive(false);
        }

        public void SwitchOffUpBtn()
        {
            btnUp.gameObject.SetActive(false);
        }

        public void OnButtonClicked(ElevatorDirection direction)
        {
            elevator.AddRequest(Num, direction);
        }

        private void OnFloorChanged(int floorNum)
        {
            floorDisplay.OnFloorChanged(floorNum);
        }

        private void OnEnteredIdle()
        {
            floorDisplay.OnEnteredIdle();
        }

        private void OnGoalFloorReached(ElevatorController.Request request)
        {
            SetBtnsState(request);
        }

        private void OnRequestNoLongerActual(ElevatorController.Request request)
        {
            SetBtnsState(request);
        }

        private void SetBtnsState(ElevatorController.Request request)
        {
            if (request.FloorNum != Num)
            {
                return;
            }

            floorDisplay.OnGoalFloorReached();
            btnUp.OnGoalFloorReached(request.Direction);
            btnDown.OnGoalFloorReached(request.Direction);
        }
    }
}



