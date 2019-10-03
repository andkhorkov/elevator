using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cabin
{
    public class Btn : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Controller cabin;
        [SerializeField] private int floorNum;

        private Image sprBtn;

        private void Awake()
        {
            sprBtn = GetComponent<Image>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            cabin.OnButtonClicked(floorNum);
            sprBtn.color = Color.red;
        }
    }
}


