using UnityEngine;

namespace Floor
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftDoor;
        [SerializeField] private SpriteRenderer rightDoor;
        [SerializeField] private float cycleTime = 10;
        [SerializeField] private float speed = 50;

        private float currentTime;

        public Vector2 DoorSize => leftDoor.bounds.size;
    }
}

