using UnityEngine;

namespace Cabin
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Display display;

        private ElevatorController elevator;

        public void OnButtonClicked(int floorNum)
        {
            Debug.Log($"cabin: {floorNum}");
        }

        public void Initialize(ElevatorController elevator)
        {
            this.elevator = elevator;

            elevator.FloorChanged += OnFloorChanged;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
        }

        private void OnFloorChanged(int floorNum)
        {
            display.OnFloorChanged(floorNum);
        }
    }
}

