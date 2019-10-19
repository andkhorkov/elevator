using Pool;
using UnityEngine;

namespace Cabin
{
    public class CabinController : PoolObject
    {
        [SerializeField] private CabinDisplay cabinDisplay;
        [SerializeField] private CanvasGroup cg;
        [SerializeField] private float fadeInTime = 0.25f;
        [SerializeField] private CabinBtn[] btns;

        private ElevatorController elevator;
        private bool IsVisible;
        private static float fadeDelta;

        private void Awake()
        {
            ElevatorController.GoalFloorReached += OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual += OnRequestNoLongerActual;
            GameController.Restart += OnRestart;

            fadeDelta = 1 / fadeInTime;
        }

        private void OnDestroy()
        {
            elevator.FloorChanged -= OnFloorChanged;
            ElevatorController.GoalFloorReached -= OnGoalFloorReached;
            ElevatorController.RequestNoLongerActual -= OnRequestNoLongerActual;
            GameController.Restart -= OnRestart;
        }

        private void OnRestart()
        {
            ReturnObject();
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

        public override void OnTakenFromPool()
        {
            
        }

        public override void OnReturnedToPool()
        {
            transform.position = Vector3.right * 10000;
            name = "pooledCabin";
        }

        public override void OnPreWarmed()
        {
        }
    }
}

