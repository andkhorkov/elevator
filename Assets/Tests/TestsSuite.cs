using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Tests
{
    public class TestsSuite
    {
        private GameObject world;
        private ElevatorController elevator;
        private List<int> visitedFloors = new List<int>();

        [SetUp]
        public void Setup()
        {
            var cam = new GameObject
            {
                tag = "MainCamera"
            };

            cam.AddComponent<Camera>();
            world = Object.Instantiate(Resources.Load<GameObject>("world"));
            Time.timeScale = 100;
        }

        private void OnElevatorReachGoalFloor(int floorNum, ElevatorDirection direction)
        {
            visitedFloors.Add(floorNum);
        }

        private void SetElevator()
        {
            elevator = world.GetComponentInChildren<ElevatorController>();
            elevator.GoalFloorReached += OnElevatorReachGoalFloor;
        }

        [TearDown]
        public void Teardown()
        {
            Object.Destroy(world.gameObject);
            Time.timeScale = 1;
            visitedFloors.Clear();
        }

        [UnityTest]
        public IEnumerator GameSetupComplete()
        {
            yield return null;
            Assert.IsNotNull(world);
        }

        [UnityTest]
        public IEnumerator Elevator_GoingUpCurrentTaskDownRequestedDown_HigherEarlier()
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return null;
            yield return new AwaitUntilElevatorReachFloor(2, elevator);
            elevator.Floors[4].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(4, elevator);
            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[2].BtnDown.OnClick();
            yield return null;

            yield return new AwaitUntilNumberOfApproachedFloorsIs(4, visitedFloors);

            Assert.AreEqual(new List<int>() { 6, 4, 3, 2 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_SameDirTasksWhenTheyLowerAndWeHaveSomeOppositeTasksAlready_Postponing()
        {
            SetElevator();

            elevator.Floors[6].BtnUp.OnClick();
            yield return null;
            elevator.Floors[5].BtnUp.OnClick();
            yield return null;
            elevator.Floors[3].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(2, elevator);
            elevator.Floors[1].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(4, visitedFloors);

            Assert.AreEqual(new List<int>() { 5,6,3,1 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_2TasksDownFirstThen2TasksUp_DownDescendingUpAscending()
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[5].BtnDown.OnClick();
            yield return new WaitForSeconds(0.5f);
            elevator.Floors[4].BtnUp.OnClick();
            yield return new WaitForSeconds(0.2f);
            elevator.Floors[3].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(4, visitedFloors);

            Assert.AreEqual(new List<int>() { 6, 5, 3, 4 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_2TasksUpFirstThen2TasksDown_UpAscendingDownDescending()
        {
            SetElevator();

            elevator.Floors[5].BtnUp.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[3].BtnUp.OnClick();
            yield return new WaitForSeconds(0.5f);
            elevator.Floors[4].BtnDown.OnClick();
            yield return new WaitForSeconds(0.2f);
            elevator.Floors[6].BtnDown.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(4, visitedFloors);

            Assert.AreEqual(new List<int>() { 3, 5, 6, 4 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_GoesUpForDownTasksWhileAcceptingTasksUp_UpTasksLater()
        {
            SetElevator();

            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[6].BtnDown.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[4].BtnDown.OnClick();
            yield return new WaitForSeconds(0.8f);
            elevator.Floors[1].BtnUp.OnClick();
            yield return new WaitForSeconds(0.4f);
            elevator.Floors[2].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(5, visitedFloors);

            Assert.AreEqual(new List<int>() { 6, 4, 3, 1, 2 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_DuplicateTasks_NotAllowed()
        {
            SetElevator();

            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[4].BtnDown.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(0.8f);
            elevator.Floors[1].BtnUp.OnClick();
            yield return new WaitForSeconds(0.4f);
            elevator.Floors[1].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(3, visitedFloors);

            Assert.AreEqual(new List<int>() { 4, 3, 1 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_1TaskUpFirstThen1TaskDownSameFloor_TwoEventsSameDoorCycle()
        {
            SetElevator();

            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[3].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(2, visitedFloors); // two events: first on arrive, second when doors closed and nobody requested with direction aligned with first request

            Assert.AreEqual(new List<int>() { 3, 3 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_1TaskUpFirstThen1TaskDownSameFloorThen1TaskUp_TwoEventsSameDoorCycleThenGoUp()
        {
            SetElevator();

            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[3].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[4].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(3, visitedFloors);

            Assert.AreEqual(new List<int>() { 3, 3, 4 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_OneTaskDownFirstThenOtherTasksUp_DownTaskFirstThenUpInAcending()
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(6, elevator);
            elevator.Floors[1].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[3].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[5].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[2].BtnUp.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(5, visitedFloors);

            Assert.AreEqual(new List<int>() { 6, 1, 2, 3, 5 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_RequestingDownWhileDoorsAreClosingTheSameFloor_CantExplain() // should've probably better count amount of times the doors opened
        {
            SetElevator();

            elevator.Floors[5].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(5, elevator);
            elevator.Floors[4].BtnUp.OnClick();
            yield return new AwaitUntilElevatorReachFloor(4, elevator);
            yield return new WaitForSeconds(0.5f);
            elevator.Floors[4].BtnDown.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[3].BtnDown.OnClick();

            yield return new AwaitUntilNumberOfApproachedFloorsIs(4, visitedFloors);

            Assert.AreEqual(new List<int>() { 5, 4, 4, 3 }, visitedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_WTF_CantExplain() 
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(6, elevator);
            elevator.Floors[3].BtnDown.OnClick();
            yield return new AwaitUntilElevatorReachFloor(4, elevator);
            elevator.Floors[5].BtnDown.OnClick();
            yield return new WaitForSeconds(0.5f);
            elevator.Floors[2].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[4].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            yield return new AwaitUntilElevatorReachFloor(3, elevator);
            elevator.Floors[6].BtnUp.OnClick(); // on 3rd floor he lied end went to 6th (emulated cabin btn)

            yield return new AwaitUntilNumberOfApproachedFloorsIs(6, visitedFloors);

            Assert.AreEqual(new List<int>() { 6, 3, 2, 4, 6, 5 }, visitedFloors);
        }

        private void Print()
        {
            var s = new StringBuilder();

            foreach (var floor in visitedFloors)
            {
                s.Append($"{floor}, ");
            }

            Debug.Log(s.ToString());
        }
    }

    public class AwaitUntilElevatorReachFloor : CustomYieldInstruction
    {
        private int requiredFloorNum;
        private ElevatorController elevator;

        public AwaitUntilElevatorReachFloor(int requiredFloorNum, ElevatorController elevator)
        {
            this.requiredFloorNum = requiredFloorNum;
            this.elevator = elevator;
        }

        public override bool keepWaiting => elevator.CurrFloorNum != requiredFloorNum;
    }

    public class AwaitUntilNumberOfApproachedFloorsIs : CustomYieldInstruction
    {
        private int requiredNumOfVisitedFloors;
        private List<int> visitedFloors;

        public AwaitUntilNumberOfApproachedFloorsIs(int requiredNumOfVisitedFloors, List<int> visitedFloors)
        {
            this.requiredNumOfVisitedFloors = requiredNumOfVisitedFloors;
            this.visitedFloors = visitedFloors;
        }

        public override bool keepWaiting => visitedFloors.Count != requiredNumOfVisitedFloors;
    }
}
