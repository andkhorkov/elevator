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
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.EnteredIdle -= OnEnteredIdle;
            elevator.DirectionChanged -= OnDirectionChanged;
            elevator.GoalFloorReached -= OnGoalFloorReached;
        }

        public void OnDirectionChanged(ElevatorDirection direction)
        {
            floorDisplay.OnDirectionChanged(direction);
        }

        public void OpenDoors()
        {
            if (Num != elevator.CurrentFloorNum)
            {
                Debug.LogWarning("trying to kill people");
                return;
            }

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

        public void OnGoalFloorReached(int floorNum, ElevatorDirection direction)
        {
            if (floorNum != Num)
            {
                return;
            }
            
            floorDisplay.OnGoalFloorReached();
            btnUp.OnGoalFloorReached(direction);
            btnDown.OnGoalFloorReached(direction);
        }
    }
}



