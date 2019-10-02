using UnityEngine;

public enum ElevatorDirection
{
    up,
    down
}

public class FloorBtn : MonoBehaviour, IClickable
{
    [SerializeField] private FloorController floor;
    [SerializeField] private ElevatorDirection direction;

    private SpriteBtn btn;

    private void Awake()
    {
        btn = GetComponent<SpriteBtn>();
    }

    public void OnClick()
    {
        floor.OnButtonClicked(direction);
    }
}
