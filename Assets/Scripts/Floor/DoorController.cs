using UnityEngine;

namespace Floor
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer leftDoor;
        [SerializeField] private SpriteRenderer rightDoor;
        [SerializeField] private float cycleTime = 10;
        [SerializeField] private Animator animator;

        private int isOpened;
        private float currentTime;

        public Vector2 DoorSize => leftDoor.bounds.size;

        private void Awake()
        {
            isOpened = Animator.StringToHash("isOpened");
        }

        public void Open()
        {
            animator.SetBool(isOpened, true); // didn't figured common name for open/close(bool)
        }

        public void Close()
        {
            animator.SetBool(isOpened, false);
        }
    }
}

