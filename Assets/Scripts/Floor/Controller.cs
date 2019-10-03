using UnityEngine;

namespace Floor
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftDoor;
        [SerializeField] private SpriteRenderer rightDoor;
        [SerializeField] private Display display;
        [SerializeField] private Btn btnUp;
        [SerializeField] private Btn btnDown;
        private ElevatorController elevator;

        public int Id { get; private set; }

        public Vector2 Size => leftDoor.sprite.bounds.size;

        public void Initialize(int id, ElevatorController elevator)
        {
            Id = id;
            this.elevator = elevator;

            elevator.FloorChanged += OnFloorChanged;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
        }

        public void OnButtonClicked(ElevatorDirection direction)
        {
            Debug.Log($"{Id} : {direction}");
        }

        private void OnFloorChanged(int floorNum)
        {
            display.OnFloorChanged(floorNum);
        }
    }
}



