using UnityEngine;

namespace Floor
{
    public class FloorController : MonoBehaviour
    {
        [SerializeField] private DoorController doorController;
        [SerializeField] private Display display;
        [SerializeField] private Btn btnUp;
        [SerializeField] private Btn btnDown;

        private ElevatorController elevator;

        public DoorController DoorController => doorController;

        public int Num { get; private set; }

        public Vector3 Position { get; private set; }

        public void Initialize(int num, ElevatorController elevator)
        {
            Num = num;
            Position = transform.position;
            this.elevator = elevator;

            elevator.FloorChanged += OnFloorChanged;
        }

        public void UpdateDoors()
        {

        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
        }

        public void OnButtonClicked(ElevatorDirection direction)
        {
            Debug.Log($"{Num} : {direction}");
        }

        private void OnFloorChanged(int floorNum)
        {
            display.OnFloorChanged(floorNum);
        }
    }
}



