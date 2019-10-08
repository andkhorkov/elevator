using System;
using System.Collections;
using System.Collections.Generic;
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
        List<int> opproachedFloors = new List<int>();

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
            opproachedFloors.Add(floorNum);
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
            opproachedFloors.Clear();
        }

        [UnityTest]
        public IEnumerator GameSetupComplete()
        {
            yield return null;
            Assert.IsNotNull(world);
        }

        private IEnumerator AwaitUntilElevatorReachFloor(int floorNum)
        {
            while (elevator.CurrentFloorNum != floorNum)
            {
                yield return null;
            }
        }

        private IEnumerator AwaitUntilNumberOfOpproachedFloorsIs(int num)
        {
            while (opproachedFloors.Count != num)
            {
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator Elevator_GoingUpCurrentTaskDownRequestedDown_HigherEarlier()
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return null;
            yield return AwaitUntilElevatorReachFloor(2);
            elevator.Floors[4].BtnDown.OnClick();
            yield return AwaitUntilElevatorReachFloor(4);
            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[2].BtnDown.OnClick();
            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(4);

            Assert.AreEqual(new List<int>() { 6, 4, 3, 2 }, opproachedFloors);
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
            yield return AwaitUntilElevatorReachFloor(2);
            elevator.Floors[1].BtnUp.OnClick();

            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(4);

            Assert.AreEqual(new List<int>() { 5,6,3,1 }, opproachedFloors);
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

            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(4);

            Assert.AreEqual(new List<int>() { 6, 5, 3, 4 }, opproachedFloors);
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

            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(4);

            Assert.AreEqual(new List<int>() { 3, 5, 6, 4 }, opproachedFloors);
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

            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(5);

            Assert.AreEqual(new List<int>() { 6, 4, 3, 1, 2 }, opproachedFloors);
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

            yield return null;

            yield return AwaitUntilNumberOfOpproachedFloorsIs(3);

            Assert.AreEqual(new List<int>() { 4, 3, 1 }, opproachedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_1TaskUpFirstThen1TaskDownSameFloor_TwoEventsSameDoorCycle()
        {
            SetElevator();

            elevator.Floors[3].BtnDown.OnClick();
            yield return new WaitForSeconds(2);
            elevator.Floors[3].BtnUp.OnClick();

            yield return AwaitUntilNumberOfOpproachedFloorsIs(2); // two events: first on arrive, second when doors closed and nobody requested with direction aligned with first request

            Assert.AreEqual(new List<int>() { 3, 3 }, opproachedFloors);
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

            yield return AwaitUntilNumberOfOpproachedFloorsIs(3);

            Assert.AreEqual(new List<int>() { 3, 3, 4 }, opproachedFloors);
        }

        [UnityTest]
        public IEnumerator Elevator_1TaskUpFirstThen1TaskDownSameFloorThen1TaskUp_NotAllowed()
        {
            SetElevator();

            elevator.Floors[6].BtnDown.OnClick();
            yield return AwaitUntilElevatorReachFloor(6);
            elevator.Floors[1].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[3].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[5].BtnUp.OnClick();
            yield return new WaitForSeconds(1);
            elevator.Floors[2].BtnUp.OnClick();

            yield return AwaitUntilNumberOfOpproachedFloorsIs(3);

            Assert.AreEqual(new List<int>() { 6, 1, 2, 3, 5 }, opproachedFloors);
        }
    }
}
