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

            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;

            fadeDelta = 1 / fadeInTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            elevator.FloorChanged -= OnFloorChanged;
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
                btns[i].SetDefaultColor();
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
            btns[floorNum - 1].SetDefaultColor();
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

