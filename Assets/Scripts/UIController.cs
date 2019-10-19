using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private InputField elevatorsCountInput;
    [SerializeField] private InputField floorsCountInput;
    [SerializeField] private Button restartBtn;

    private int maxElevators;
    private int maxFloors;

    public InputField ElevatorsCountInput => elevatorsCountInput;
    public InputField FloorsCountInput => floorsCountInput;
    public Button RestartBtn => restartBtn;

    private void Awake()
    {
        GameController.Initialized += Initialize;
    }

    private void OnDestroy()
    {
        GameController.Initialized -= Initialize;
    }

    public void SetLimits(int maxElevators, int maxFloors)
    {
        this.maxElevators = maxElevators;
        this.maxFloors = maxFloors;
    }

    private void Initialize()
    {
        
    }

    
}
