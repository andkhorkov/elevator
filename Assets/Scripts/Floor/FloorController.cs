using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer leftDoor;
    [SerializeField] private SpriteRenderer rightDoor;
    [SerializeField] private FloorDisplay display;
    [SerializeField] private FloorButtons btns;

    public int Id { get; private set; }

    public Vector2 Size => leftDoor.sprite.bounds.size;

    public void Initialize(int id)
    {
        Id = id;
    }

    public void OnButtonClicked(ElevatorDirection direction)
    {
        Debug.Log($"{Id} : {direction}");
    }
}

