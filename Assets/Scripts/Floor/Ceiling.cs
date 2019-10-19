using Pool;
using UnityEngine;

public class Ceiling : PoolObject
{
    public SpriteRenderer Spr { get; private set; }

    private void Awake()
    {
        Spr = GetComponent<SpriteRenderer>();

        GameController.Restart += OnRestart;
    }

    private void OnDestroy()
    {
        GameController.Restart -= OnRestart;
    }

    private void OnRestart()
    {
        ReturnObject();
    }

    public override void OnTakenFromPool()
    {
    }

    public override void OnReturnedToPool()
    {
        transform.position = Vector3.right * 10000;
        name = "pooledCeiling";
    }

    public override void OnPreWarmed()
    {
    }
}
