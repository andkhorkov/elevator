using System;
using UnityEngine;

public class ElevatorCreator : MonoBehaviour
{
    [SerializeField] private int numFloors = 6;
    [SerializeField] private int numElevators = 1;
    [SerializeField] private float ceilToDoorOffset = 10;
    [SerializeField] private Vector2 desiredResolution = new Vector2(2880, 1800);
    [SerializeField] private Transform doorsTf;
    [SerializeField] private SpriteRenderer wall;

    private Vector3 origin;

    public static event Action ElevatorInitialized = delegate {  };

    private void Start()
    {
        SetCameraSize();

        var floorPositions = BuildFloors();
        BuildElevators(floorPositions);
        ElevatorInitialized.Invoke();
    }

    private Vector3[] BuildFloors()
    {
        var ceiling = Resources.Load<SpriteRenderer>("ceiling");
        var doorsPrefab = Resources.Load<Floor>("floorElevatorDoors");
        var doorSize = doorsPrefab.Size;
        var floorPositions = new Vector3[numFloors + 1];

        origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
        origin.z = 0;
        doorsTf.position = new Vector3(doorsTf.position.x, origin.y + doorSize.y * 0.5f);
        var offset = desiredResolution.x * 0.5f * Vector3.right;
        var ceilSize = new Vector2(desiredResolution.x, 10);
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
        var doorsPrefab = Resources.Load<Floor>("floorElevatorDoors");
        var cabinPrefab = Resources.Load("elevatorCabin");

        for (int i = 0; i < numElevators; i++)
        {
            var elevator = new GameObject($"elevator{i + 1}");
            var elevatorPos = origin + 700 * Vector3.right * (i + 1);
            elevator.transform.position = elevatorPos;
            elevator.transform.SetParent(transform);
            var floors = new Floor[numFloors];

            for (int j = 0; j < numFloors; j++)
            {
                var floor = Instantiate(doorsPrefab, elevator.transform);
                var id = j + 1;
                floor.Initialize(id);
                floor.transform.position = new Vector3(elevatorPos.x, floorPositions[j].y);
                floor.name = $"doors{id}";
                floors[j] = floor;
            }

            var cabin = (GameObject) Instantiate(cabinPrefab, elevator.transform);
            cabin.transform.position = new Vector3(elevatorPos.x, floorPositions[0].y);
            cabin.name = $"cabin{i + 1}";

            var elevatorController = elevator.AddComponent<ElevatorController>();
            elevatorController.Initialize(floors);
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
}
