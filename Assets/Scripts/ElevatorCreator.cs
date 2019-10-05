using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorCreator : MonoBehaviour
{
    [SerializeField] private int numFloors = 6;
    [SerializeField] private int numElevators = 1;
    [SerializeField] private float ceilToDoorOffset = 10;
    [SerializeField] private float ceilWidth = 10;
    [SerializeField] private Vector2 desiredResolution = new Vector2(2880, 1800);
    [SerializeField] private Transform doorsTf;
    [SerializeField] private SpriteRenderer wall;

    private Vector3 origin;

    public static event Action<ElevatorController> ElevatorInitialized = delegate {  };

    private void Start()
    {
        SetCameraSize();

        var floorPositions = BuildFloors();
        BuildElevators(floorPositions);
    }

    private Vector3[] BuildFloors()
    {
        var ceiling = Resources.Load<SpriteRenderer>("ceiling");
        var floorPrefab = Resources.Load<Floor.FloorController>("floorController");
        var doorSize = floorPrefab.DoorController.DoorSize;
        var floorPositions = new Vector3[numFloors + 1];

        origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        origin.z = 0;
        doorsTf.position = new Vector3(doorsTf.position.x, origin.y + doorSize.y * 0.5f);
        var offset = desiredResolution.x * 0.5f * Vector3.right;
        var ceilSize = new Vector2(desiredResolution.x, ceilWidth);
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

    private void BuildElevators(Vector3[] floorPositions)
    {
        var floorControllerPrefab = Resources.Load<Floor.FloorController>("floorController");
        var cabinPrefab = Resources.Load<Cabin.CabinController>("elevatorCabin");

        for (int i = 0; i < numElevators; i++)
        {
            var elevator = new GameObject($"elevator{i + 1}");
            var elevatorController = elevator.AddComponent<ElevatorController>();
            var elevatorPos = origin + 700 * Vector3.right * (i + 1);
            elevator.transform.position = elevatorPos;
            elevator.transform.SetParent(transform);
            var floors = new Dictionary<int, Floor.FloorController>(numFloors);

            for (int j = 0; j < numFloors; j++)
            {
                var floor = Instantiate(floorControllerPrefab, elevator.transform);
                var floorNum = j + 1;
                floor.transform.position = new Vector3(elevatorPos.x, floorPositions[j].y + 0.5f * ceilWidth);
                floor.name = $"doors{floorNum}";
                floors[floorNum] = floor;
                floor.Initialize(floorNum, elevatorController);
            }

            var cabin = Instantiate(cabinPrefab, elevator.transform);
            cabin.transform.position = new Vector3(elevatorPos.x, floorPositions[0].y);
            cabin.name = $"cabin{i + 1}";
            cabin.Initialize(elevatorController);
            
            elevatorController.Initialize(floors, cabin);
        }
    }

    private void SetCameraSize()
    {
        float nativeToRealRatio = desiredResolution.x / Screen.width;
        Camera.main.orthographicSize = 0.5f * nativeToRealRatio * Screen.height;
        wall.size = new Vector2(wall.size.x, wall.size.x / Camera.main.aspect);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(Camera.main.ScreenToWorldPoint(Vector3.zero), 100);
    //}

    void Update()
    {
        float unitsPerPixel = 2880f / Screen.width;

        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;

        Camera.main.orthographicSize = desiredHalfHeight;

    }
}
