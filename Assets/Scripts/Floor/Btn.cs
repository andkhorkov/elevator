﻿using UnityEngine;

namespace Floor
{
    public class Btn : MonoBehaviour, IClickable
    {
        [SerializeField] private FloorController floor;
        [SerializeField] private ElevatorDirection direction;

        private SpriteBtn sprBtn;

        private void Awake()
        {
            sprBtn = GetComponent<SpriteBtn>();
        }

        public void OnClick()
        {
            floor.OnButtonClicked(direction);
            sprBtn.SprRenderer.color = Color.red;
        }
    }
}