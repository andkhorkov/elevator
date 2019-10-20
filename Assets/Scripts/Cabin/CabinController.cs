using Elevator;
using UnityEngine;

namespace Cabin
{
    public class CabinController : ElevatorElement
    {
        [SerializeField] private CabinDisplay cabinDisplay;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private float fadeInTime = 0.25f;
        [SerializeField] private CabinBtn[] btns;

        private ElevatorController elevator;
        private bool IsVisible;
        private static float fadeDelta;

        protected override void Awake()
        {
            base.Awake();

            fadeDelta = 1 / fadeInTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Unsubscribes();
        }

        protected override void Unsubscribes()
        {
            elevator.FloorChanged -= OnFloorChanged;
            elevator.FloorRequested -= OnFloorRequested;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual -= OnRequestNoLongerActual;
        }

        protected override void OnRestart()
        {
            base.OnRestart();

            cg.alpha = 0;
            IsVisible = false;

            for (int i = 0; i < btns.Length; i++)
            {
                btns[i].Disactivate();
            }
        }

        public void OnButtonClicked(int floorNum)
        {
            elevator.AddRequest(floorNum, ElevatorDirection.none);
        }

        public void Initialize(ElevatorController elevator)
        {
            this.elevator = elevator;

            elevator.FloorChanged += OnFloorChanged;
            elevator.FloorRequested += OnFloorRequested;
            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;
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

            DisactivateBtn(request.FloorNum);
        }

        private void OnRequestNoLongerActual(ElevatorController.Request request, ElevatorController elevator)
        {
            if (this.elevator != elevator)
            {
                return;
            }

            DisactivateBtn(request.FloorNum);
        }

        private void DisactivateBtn(int floorNum)
        {
            btns[floorNum - 1].Disactivate();
        }

        private void OnFloorRequested(int floorNum)
        {
            btns[floorNum - 1].Activate();
        }

        private void OnFloorChanged(int floorNum)
        {
            cabinDisplay.OnFloorChanged(floorNum);
        }

        private void Update()
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, IsVisible ? 1 : 0, fadeDelta * Time.deltaTime);
        }
    }
}

