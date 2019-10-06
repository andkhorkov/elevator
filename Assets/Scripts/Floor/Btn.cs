using UnityEngine;

namespace Floor
{
    public class Btn : MonoBehaviour, IClickable
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
            floor.OnButtonClicked(direction);
            sprBtn.SprRenderer.color = Color.red;
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