using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloorDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI lblCurrentFloor;
    [SerializeField] private CanvasGroup sprDirection;
    [SerializeField] private CanvasGroup sprHere;

    private void Awake()
    {
        ElevatorCreator.ElevatorInitialized += OnElevatorInitialized;
    }

    private void OnDestroy()
    {
        ElevatorCreator.ElevatorInitialized -= OnElevatorInitialized;
    }

    private void OnElevatorInitialized()
    {

    }
}
