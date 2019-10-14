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
            elevator.GoalFloorReached += OnGoalFloorReached;
            elevator.RequestNoLongerActual += OnRequestNoLongerActual;
        }

        public void ShowCabin(bool show)
        {
            IsVisible = show;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.GoalFloorReached -= OnGoalFloorReached;
            elevator.RequestNoLongerActual -= OnRequestNoLongerActual;
        }

        private void OnGoalFloorReached(ElevatorController.Request request)
        {
            SetBtnState(request);
        }

        private void OnRequestNoLongerActual(ElevatorController.Request request)
        {
            SetBtnState(request);
        }

        private void SetBtnState(ElevatorController.Request request)
        {
            btns[request.FloorNum - 1].Reset();
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