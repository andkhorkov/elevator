using System;
using System.Collections.Generic;
using Floor;
using UnityEngine;

public class BasementController : MonoBehaviour
{
    [SerializeField] private int elevatorSpeed = 400;
    [SerializeField] private float shaftOffset = 800;
    [SerializeField] private float shaftWidth = 800;
    [SerializeField] private int numFloors = 6;
    [SerializeField] private int numElevators = 2;
    [SerializeField] private float ceilToDoorOffset = 10;
    [SerializeField] private float ceilWidth = 10;
    [SerializeField] private Vector2 desiredResolution = new Vector2(2880, 1800);
    [SerializeField] private SpriteRenderer wall;

    private ElevatorController[] elevators;

    public static event Action<ElevatorController> ElevatorInitialized = delegate {  };

    public void SetNumElevators(int numElevators) //for tests
    {
        this.numElevators = numElevators;
    }

    private void Start()
    {
        var origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        origin.z = 0;
        var floorPositions = BuildCeilings(origin);
        BuildElevators(floorPositions, origin);
    }

    private Vector3[] BuildCeilings(Vector3 origin)
    {
        wall.size = new Vector2(desiredResolution.y * Camera.main.aspect, desiredResolution.y);
        var ceiling = Resources.Load<SpriteRenderer>("ceiling");
        var floorPrefab = Resources.Load<FloorController>("floorController");
        var doorSize = floorPrefab.DoorController.DoorSize;
        var floorPositions = new Vector3[numFloors + 1];
        var offset = wall.size.x * 0.5f * Vector3.right;
        var ceilSize = new Vector2(wall.size.x, ceilWidth);
        var firstFloorPos = origin + offset;

        for (int i = 0; i < floorPositions.Length; ++i)
        {
            floorPositions[i] = firstFloorPos + Vector3.up * ((doorSize.y + ceilToDoorOffset) * i + ceilSize.y * (0.5f + i));
            var ceilingSpr = Instantiate(ceiling, floorPositions[i], Quaternion.identity);
            ceilingSpr.transform.SetParent(transform);
            ceilingSpr.size = ceilSize;
            ceilingSpr.name = $"floor{i + 1}";
        }

        return floorPositions;
    }

    private void BuildElevators(Vector3[] floorPositions, Vector3 origin)
    {
        elevators = new ElevatorController[numElevators];
        var floorControllerPrefab = Resources.Load<FloorController>("floorController");
        var cabinPrefab = Resources.Load<Cabin.CabinController>("elevatorCabin");

        for (int i = 0; i < numElevators; i++)
        {
            var elevatorNum = i + 1;
            var elevator = new GameObject($"elevator{elevatorNum}");
            var elevatorController = elevator.AddComponent<ElevatorController>();
            var elevatorPos = origin + (shaftOffset + shaftWidth * i) * Vector3.right;
            elevator.transform.position = elevatorPos;
            elevator.transform.SetParent(transform);
            var floors = new Dictionary<int, FloorController>(numFloors);

            for (int j = 0; j < numFloors; j++)
            {
                var floor = Instantiate(floorControllerPrefab, elevator.transform);
                var floorNum = j + 1;
                floor.transform.position = new Vector3(elevatorPos.x, floorPositions[j].y + 0.5f * ceilWidth);
                floor.name = $"doors{floorNum}";
                floors[floorNum] = floor;
                floor.Initialize(floorNum, elevatorController, this);
            }

            floors[1].SwitchOffDownBtn();
            floors[numFloors].SwitchOffUpBtn();

            var cabin = Instantiate(cabinPrefab, elevator.transform);
            cabin.transform.position = new Vector3(elevatorPos.x, floorPositions[0].y);
            cabin.name = $"cabin{i + 1}";
            cabin.Initialize(elevatorController);
            
            elevatorController.Initialize(floors, cabin, elevatorSpeed, elevatorNum);
            elevators[i] = elevatorController;
        }
    }

    public void AddRequest(int desiredFloorNum, ElevatorDirection desiredDirection)
    {
        var request = new ElevatorController.Request(desiredDirection, desiredFloorNum);
        var elevator = GetClosestElevator(request);

        if (elevator == null)
        {
            return;
        }
        
        elevator.AddRequest(desiredFloorNum, desiredDirection);
    }

    private ElevatorController GetClosestElevator(ElevatorController.Request request)
    {
        var sameDirAndIdleElevators = new List<ElevatorController>();
        var otherElevators = new List<ElevatorController>();

        for (int i = 0; i < elevators.Length; i++)
        {
            var elevator = elevators[i];

            if (elevator.IsRequestExists(request))
            {
                return null;
            }

            if (elevator.IsIdle || elevator.MovingDirection == request.Direction)
            {
                sameDirAndIdleElevators.Add(elevator);
                continue;
            }

            otherElevators.Add(elevator);
        }

        if (sameDirAndIdleElevators.Count > 0)
        {
            sameDirAndIdleElevators.Sort((a, b) => Mathf.Abs(a.CurrFloorNum - request.FloorNum).CompareTo(Mathf.Abs(b.CurrFloorNum - request.FloorNum)));
            return sameDirAndIdleElevators[0];
        }

        otherElevators.Sort((a, b) => a.RequestsCount.CompareTo(b.RequestsCount));
        return otherElevators[0];
    }
}
