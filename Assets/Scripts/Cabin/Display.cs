﻿using TMPro;
using UnityEngine;

namespace Cabin
{
    public class Display : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI lblCurrentFloor;

        private CanvasGroup cg;

        private void Awake()
        {
            cg = GetComponent<CanvasGroup>();
        }

        public void OnFloorChanged(int floorNum)
        {
            lblCurrentFloor.text = floorNum.ToString();
        }
    }
}


