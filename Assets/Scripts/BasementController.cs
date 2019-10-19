using System;
using System.Collections.Generic;
using Cabin;
using Floor;
using Pool;
using UnityEngine;

public class BasementController : MonoBehaviour
{
    [SerializeField] private int elevatorSpeed = 400;
    [SerializeField] private float shaftOffset = 800;
    [SerializeField] private float shaftWidth = 800;
    [SerializeField] private float ceilToDoorOffset = 10;
    [SerializeField] private float ceilWidth = 10;
    [SerializeField] private Vector2 desiredResolution = new Vector2(2880, 1800);
    [SerializeField] private SpriteRenderer wall;
    [SerializeField] private GameController gameController;
    [SerializeField, AssetPathGetter] private string ceilingPrefabPath;
    [SerializeField, AssetPathGetter] private string floorPrefabPath;
    [SerializeField, AssetPathGetter] private string cabinPrefabPath;

    private List<ElevatorController> sameDirAndIdleElevators = new List<ElevatorController>();
    private List<ElevatorController> otherElevators = new List<ElevatorController>();

    private ElevatorController[] elevators;
    private Vector3 origin;

    private void Start()
    {
        origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        origin.z = 0;
        wall.size = new Vector2(desiredResolution.y * Camera.main.aspect, desiredResolution.y);
    }

    public void Restart(int elevatorsCount, int floorsCount)
    {
        var floorPositions = BuildCeilings(origin, floorsCount);
        BuildElevators(floorPositions, origin, elevatorsCount, floorsCount);
    }

    private Vector3[] BuildCeilings(Vector3 origin, int floorsCount)
    {
        var floorPrefab = Resources.Load<FloorController>("floorController");
        var doorSize = floorPrefab.DoorController.DoorSize;
        var floorPositions = new Vector3[floorsCount + 1];
        var offset = wall.size.x * 0.5f * Vector3.right;
        var ceilSize = new Vector2(wall.size.x, ceilWidth);
        var firstFloorPos = origin + offset;

        for (int i = 0; i < floorPositions.Length; ++i)
        {
            floorPositions[i] = firstFloorPos + Vector3.up * ((doorSize.y + ceilToDoorOffset) * i + ceilSize.y * (0.5f + i));
            var ceilingSpr = PoolManager.GetObject<Ceiling>(ceilingPrefabPath);
            ceilingSpr.SetOrientation(floorPositions[i], Quaternion.identity);
            ceilingSpr.transform.SetParent(transform);
            ceilingSpr.Spr.size = ceilSize;
            ceilingSpr.name = $"floor{i + 1}";
        }

        return floorPositions;
    }

    private void BuildElevators(Vector3[] floorPositions, Vector3 origin, int elevatorsCount, int floorsCount)
    {
        elevators = new ElevatorController[elevatorsCount];
        var floorControllerPrefab = Resources.Load<FloorController>("floorController");

        for (int i = 0; i < elevatorsCount; i++)
        {
            var elevatorNum = i + 1;
            var elevator = new GameObject($"elevator{elevatorNum}");
            var elevatorController = elevator.AddComponent<ElevatorController>();
            var elevatorPos = origin + (shaftOffset + shaftWidth * i) * Vector3.right;
            elevator.transform.position = elevatorPos;
            elevator.transform.SetParent(transform);
            var floors = new Dictionary<int, FloorController>(floorsCount);

            for (int j = 0; j < floorsCount; j++)
            {
                var floor = PoolManager.GetObject<FloorController>(floorPrefabPath);
                floor.transform.SetParent(elevator.transform);
                var floorNum = j + 1;
                floor.transform.position = new Vector3(elevatorPos.x, floorPositions[j].y + 0.5f * ceilWidth);
                floor.name = $"doors{floorNum}";
                floors[floorNum] = floor;
                floor.Initialize(floorNum, elevatorController, this);
            }

            floors[1].SwitchOffDownBtn();
            floors[floorsCount].SwitchOffUpBtn();

            var cabin = PoolManager.GetObject<CabinController>(cabinPrefabPath);
            cabin.transform.SetParent(elevator.transform);
            cabin.transform.position = new Vector3(elevatorPos.x, floorPositions[0].y);
            cabin.name = $"cabin{i + 1}";
            cabin.Initialize(elevatorController);
            
            elevatorController.Initialize(floors, cabin, elevatorSpeed, elevatorNum);
            elevators[i] = elevatorController;
        }
    }

    public void AddRequest(int desiredFloorNum, ElevatorDirection desiredDirection, ElevatorController elevator)
    {
        var request = new ElevatorController.Request(desiredDirection, desiredFloorNum);
        var closestElevator = GetClosestElevator(request, elevator);

        if (closestElevator == null)
        {
            return;
        }
        
        closestElevator.AddRequest(desiredFloorNum, desiredDirection);
    }

    private ElevatorController GetClosestElevator(ElevatorController.Request request, ElevatorController elevator)
    {
        sameDirAndIdleElevators.Clear();
        otherElevators.Clear();

        for (int i = 0; i < elevators.Length; i++)
        {
            var el = elevators[i];

            if (el.IsRequestExists(request))
            {
                return null;
            }

            if (el.IsIdle || el.MovingDirection == request.Direction)
            {
                sameDirAndIdleElevators.Add(el);
                continue;
            }

            otherElevators.Add(el);
        }

        if (sameDirAndIdleElevators.Count > 0)
        {
            sameDirAndIdleElevators.Sort((a, b) => Mathf.Abs(a.CurrFloorNum - request.FloorNum).CompareTo(Mathf.Abs(b.CurrFloorNum - request.FloorNum)));

            var bestElevator = sameDirAndIdleElevators[0];
            var minDist = int.MaxValue;

            for (int i = 0; i < sameDirAndIdleElevators.Count; ++i)
            {
                var el = sameDirAndIdleElevators[i];

                if (el.CurrFloorNum != bestElevator.CurrFloorNum)
                {
                    break;
                }

                var dist = Mathf.Abs(el.Id - elevator.Id);

                if (dist < minDist)
                {
                    minDist = dist;
                    bestElevator = el;
                }
            }

            return bestElevator;
        }

        otherElevators.Sort((a, b) => a.RequestsCount.CompareTo(b.RequestsCount));
        return otherElevators[0];
    }
}
