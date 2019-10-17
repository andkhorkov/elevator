using UnityEngine;

namespace Cabin
{
    public class CabinController : MonoBehaviour
    {
        [SerializeField] private CabinDisplay cabinDisplay;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private float fadeInTime = 0.25f;
        [SerializeField] private CabinBtn[] btns;

        private ElevatorController elevator;
        private bool IsVisible;
        private static float maxDelta;

        public void OnButtonClicked(int floorNum)
        {
            elevator.AddRequest(floorNum, ElevatorDirection.none);
        }

        public void Initialize(ElevatorController elevator)
        {
            this.elevator = elevator;
            maxDelta = 1 / fadeInTime;

            elevator.FloorChanged += OnFloorChanged;
            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual -= OnRequestNoLongerActual;
        }

        public void ShowCabin(bool show)
        {
            IsVisible = show;
        }

        public void OnGoalFloorReached(ElevatorController.Request request, ElevatorController elevator)
        {
            if (this.elevator != elevator)
            {
                return;
            }

            SetBtnState(request.FloorNum);
        }

        private void OnRequestNoLongerActual(ElevatorController.Request request, ElevatorController elevator)
        {
            if (this.elevator != elevator)
            {
                return;
            }

            SetBtnState(request.FloorNum);
        }

        private void SetBtnState(int floorNum)
        {
            btns[floorNum - 1].Reset();
        }

        private void OnFloorChanged(int floorNum)
        {
            cabinDisplay.OnFloorChanged(floorNum);
        }

        private void Update()
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, IsVisible ? 1 : 0, maxDelta * Time.deltaTime);
        }
    }
}

