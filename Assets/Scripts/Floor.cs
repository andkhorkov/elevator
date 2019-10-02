using System;
using TMPro;
using UnityEngine;

[Serializable]
public class FloorButtons
{
    public void Initialize(Floor floor)
    {
        btnUp.Initialize(floor);
    }

    public FloorBtn btnUp;
}

[Serializable]
public class Display
{
    [SerializeField] private TextMeshProUGUI lblCurrentFloor;
    [SerializeField] private CanvasGroup sprDirection;
    [SerializeField] private CanvasGroup sprHere;

    public Display()
    {
        ElevatorCreator.ElevatorInitialized += OnElevatorInitialized;
    }

    private void OnElevatorInitialized()
    {

    }

    ~Display()
    {
        ElevatorCreator.ElevatorInitialized -= OnElevatorInitialized;
    }
}

public class Floor : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftDoor;
    [SerializeField] private SpriteRenderer rightDoor;
    [SerializeField] private Display display;
    [SerializeField] private FloorButtons floorBtns;

    private int id;

    public int Id => id;

    public Vector2 Size => leftDoor.sprite.bounds.size;

    public void Initialize(int id)
    {
        this.id = id;
        floorBtns.Initialize(this);
    }

    private void Update()
    {
        floorBtns.btnUp.Update();
    }
}
