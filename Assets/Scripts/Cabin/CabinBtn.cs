using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cabin
{
    public class CabinBtn : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
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

        public void OnPointerClick(PointerEventData eventData)
        {
            cabin.OnButtonClicked(floorNum);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            sprBtn.color = Color.red;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            sprBtn.color = defaultColor;
        }
    }
}


