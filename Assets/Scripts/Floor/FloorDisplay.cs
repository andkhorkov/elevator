using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Floor
{
    public class FloorDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lblCurrentFloor;
        [SerializeField] private Image sprDirection;
        [SerializeField] private CanvasGroup sprDirectionCg;
        [SerializeField] private CanvasGroup sprHereCg;

        private static Vector3 defaultScale;
        private static Vector3 oppositDirScale;

        private void Awake()
        {
            defaultScale = sprDirection.transform.localScale;
            oppositDirScale = Vector3.Reflect(defaultScale, Vector3.up);
        }

        public void OnEnteredIdle()
        {
            sprDirectionCg.alpha = 0;
        }

        public void OnFloorChanged(int floorNum)
        {
            lblCurrentFloor.text = floorNum.ToString();
        }

        public void OnGoalFloorReached()
        {
            sprHereCg.alpha = 1;
            sprDirectionCg.alpha = 0;
        }

        public void OnDirectionChanged(ElevatorDirection direction)
        {
            sprHereCg.alpha = 0;
            sprDirectionCg.alpha = 1;
            sprDirection.transform.localScale = direction == ElevatorDirection.up ? defaultScale : oppositDirScale;
        }
    }
}

