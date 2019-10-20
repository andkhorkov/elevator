using UnityEngine;

public class Ceiling : ElevatorElement
{
    public SpriteRenderer Spr { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Spr = GetComponent<SpriteRenderer>();
    }

    protected override void Unsubscribes()
    {
    }
}
