using TMPro;
using UnityEngine;

namespace Floor
{
    public class Display : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lblCurrentFloor;
        [SerializeField] private CanvasGroup sprDirection;
        [SerializeField] private CanvasGroup sprHere;

        public void OnFloorChanged(int floorNum)
        {
            lblCurrentFloor.text = floorNum.ToString();
        }
    }
}

