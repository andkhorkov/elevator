using UnityEngine;

namespace Floor
{
    public class FloorBtn : MonoBehaviour, IClickable
    {
        [SerializeField] private FloorController floor;
        [SerializeField] private ElevatorDirection direction;

        private SpriteBtn sprBtn;
        private Color defaultColor;

        private void Awake()
        {
            sprBtn = GetComponent<SpriteBtn>();
            defaultColor = sprBtn.SprRenderer.color;
        }

        public void OnClick()
        {
            sprBtn.SprRenderer.color = Color.red;
            floor.OnButtonClicked(direction);
        }

        public void OnGoalFloorReached(ElevatorDirection direction)
        {
            if (this.direction != direction)
            {
                return;
            }

            sprBtn.SprRenderer.color = defaultColor;
        }
    }
}