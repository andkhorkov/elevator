using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cabin
{
    public class CabinBtn : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private CabinController cabin;
        [SerializeField] private int floorNum;

        private Image sprBtn;
        private Color defaultColor;

        private void Awake()
        {
            sprBtn = GetComponent<Image>();
            defaultColor = sprBtn.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            cabin.OnButtonClicked(floorNum);
        }

        public void Activate()
        {
            sprBtn.color = Color.red;
        }

        public void Disactivate()
        {
            sprBtn.color = defaultColor;
        }
    }
}


