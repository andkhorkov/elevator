using System;
using UnityEngine;

namespace Floor
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftDoor;
        [SerializeField] private SpriteRenderer rightDoor;
        [SerializeField] private float cycleTime = 10;
        [SerializeField] private Animator animator;
        [SerializeField] private FloorController floorController;

        private int isOpened;
        private float currentTime;
        private int currentTaskIndex;

        private DelayedTask[] doorCycleTasks;

        public Vector2 DoorSize => leftDoor.bounds.size;

        private void Awake()
        {
            isOpened = Animator.StringToHash("isOpened");

            doorCycleTasks = new DelayedTask[3]
            {
                new AnimationTransitionAwaitingTask(OnDoorsOpened, "doorsOpened", animator),
                new TimeDelayedTask(OnDoorDelayTimePassed, cycleTime), 
                new AnimationTransitionAwaitingTask(OnDoorsClosed, "doorsClosed", animator)
            };
        }

        private void ToNextTask()
        {
            currentTaskIndex = ++currentTaskIndex % doorCycleTasks.Length;
        }

        private void OnDoorsOpened()
        {
            ToNextTask();
        }

        private void OnDoorDelayTimePassed()
        {
            ToNextTask();
            Close();
        }

        private void OnDoorsClosed()
        {
            ToNextTask();
            floorController.OnDoorsClosed();
        }

        public void Open()
        {
            animator.SetBool(isOpened, true); // didn't figured common name for open/close(bool)
        }

        public void Close()
        {
            animator.SetBool(isOpened, false);
        }

        public void DoorsUpdate()
        {
            doorCycleTasks[currentTaskIndex].TryExecute();
        }
    }

    public abstract class DelayedTask
    {
        protected Action task;

        protected DelayedTask(Action task)
        {
            this.task = task;
        }

        public abstract void TryExecute();
    }

    public class TimeDelayedTask : DelayedTask
    {
        private readonly float delayTime;
        private float currentTime;

        public TimeDelayedTask(Action task, float delayTime) : base(task)
        {
            this.delayTime = delayTime;
        }

        public override void TryExecute()
        {
            if (currentTime > delayTime)
            {
                task.Invoke();
                currentTime = 0;
                return;
            }

            currentTime += Time.deltaTime;
        }
    }

    public class AnimationTransitionAwaitingTask : DelayedTask
    {
        private readonly string awaitedStateName;
        private readonly Animator animator;

        public AnimationTransitionAwaitingTask(Action task, string awaitedStateName, Animator animator) : base(task)
        {
            this.awaitedStateName = awaitedStateName;
            this.animator = animator;
        }

        public override void TryExecute()
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName(awaitedStateName))
            {
                task.Invoke();
            }
        }
    }
}

