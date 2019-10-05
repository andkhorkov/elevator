using TMPro;
using UnityEngine;

namespace Floor
{
    public class Display : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lblCurrentFloor;
        [SerializeField] private CanvasGroup sprDirection;
        [SerializeField] private CanvasGroup sprHere;

        public void OnEnteredIdle()
        {
            sprDirection.alpha = 0;
        }

        public void OnFloorChanged(int floorNum)
        {
            lblCurrentFloor.text = floorNum.ToString();
        }

        public void OnGoalFloorReached()
        {
            sprHere.alpha = 1;
            sprDirection.alpha = 0;
        }
    }
}

