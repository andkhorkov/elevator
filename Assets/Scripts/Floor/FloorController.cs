using Pool;
using UnityEngine;

namespace Floor
{
    public class FloorController : PoolObject
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
        }

        private void Awake()
        {
            GameController.Restart += OnRestart;
            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.EnteredIdle -= OnEnteredIdle;
            elevator.DirectionChanged -= OnDirectionChanged;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual -= OnRequestNoLongerActual;
            GameController.Restart -= OnRestart;
        }

        private void OnRestart()
        {
            ReturnObject();

            btnUp.SetDefaultColor();
            btnDown.SetDefaultColor();
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

        public override void OnTakenFromPool()
        {
        }

        public override void OnReturnedToPool()
        {
            transform.position = Vector3.right * 10000;
            name = "pooledFloor";
        }

        public override void OnPreWarmed()
        {
        }
    }
}