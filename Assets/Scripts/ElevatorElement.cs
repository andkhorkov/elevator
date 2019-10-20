using Pool;
using UnityEngine;

public abstract class ElevatorElement : PoolObject
{
    protected virtual void Awake()
    {
        GameController.Restart += OnRestart;
    }

    protected virtual void OnDestroy()
    {
        GameController.Restart -= OnRestart;

        Unsubscribes();
    }

    protected virtual void OnRestart()
    {
        ReturnObject();
    }

    protected abstract void Unsubscribes();

    public override void OnTakenFromPool()
    {
    }

    public override void OnReturnedToPool()
    {
        transform.parent = null;
        transform.position = Vector3.right * 10000;
        name = $"pooled_{GetType()}";
        Unsubscribes();
    }

    public override void OnPreWarmed()
    {
    }
}
