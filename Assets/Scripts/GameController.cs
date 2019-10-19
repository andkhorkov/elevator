using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BasementController basement;
    [SerializeField] private UIController uiController;

    private int floorsCount = 6;
    private int elevatorsCount = 1;

    private const int MAX_ELEVATORS = 3;
    private const int MAX_FLOORS = 6;

    public static event Action Initialized = delegate {  };

    private void Awake()
    {
        uiController.ElevatorsCountInput.onValueChanged.AddListener(OnElevatorsCountChanged);
        uiController.FloorsCountInput.onValueChanged.AddListener(OnFloorsCountChanged);
        uiController.RestartBtn.onClick.AddListener(OnRestartClicked);

        Initialized.Invoke();
    }

    private void OnDestroy()
    {
        uiController.ElevatorsCountInput.onValueChanged.RemoveAllListeners();
        uiController.FloorsCountInput.onValueChanged.RemoveAllListeners();
        uiController.RestartBtn.onClick.RemoveAllListeners();
    }

    public void SetNumElevators(int elevatorsCount) //for tests
    {
        this.elevatorsCount = elevatorsCount;
    }

    private void OnElevatorsCountChanged(string value)
    {
        var amount = Check(value, MAX_ELEVATORS);

        if (amount == -1)
        {
            return;
        }

        elevatorsCount = amount;
    }

    private void OnFloorsCountChanged(string value)
    {
        var amount = Check(value, MAX_FLOORS);

        if (amount == -1)
        {
            return;
        }

        floorsCount = amount;
    }

    private static int Check(string str, int max)
    {
        if (!int.TryParse(str, out var num))
        {
            Debug.LogError($"invalid input {str}");
            return -1;
        }

        if (num < 1 || num > max)
        {
            Debug.LogError($"currently there is {max} maximum supported");
            return -1;
        }

        return num;
    }

    private void OnRestartClicked()
    {
        basement.Restart(elevatorsCount, floorsCount);
    }

    //todo: other fancy stuff here
}
