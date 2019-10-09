using UnityEngine;

namespace Floor
{
    public class FloorController : MonoBehaviour
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
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.EnteredIdle -= OnEnteredIdle;
            elevator.DirectionChanged -= OnDirectionChanged;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
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
            basement.AddRequest(Num, direction);
        }

        private void OnFloorChanged(int floorNum)
        {
            floorDisplay.OnFloorChanged(floorNum);
        }

        private void OnEnteredIdle()
        {
            floorDisplay.OnEnteredIdle();
        }

        public void OnGoalFloorReached(int floorNum, ElevatorDirection direction)
        {
            if (floorNum != Num)
            {
                return;
            }

            btnUp.OnGoalFloorReached(direction);
            btnDown.OnGoalFloorReached(direction);
        }

        public void ResetHereSign()
        {
            floorDisplay.OnGoalFloorReached();
        }
    }
}



