using System;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private BasementController basement;

    private UIController uiController;

    private int floorsAmount = 6;
    private int elevatorsAmount = 1;

    private float defaultTimeScale;
    private bool isPlaying; // could be a state machine instead
    private bool isPaused;

    public const int DEFAULT_ELEVATORS_AMOUNT = 1;
    public const int DEFAULT_FLOORS_AMOUNT = 6;
    public const int MIN_ELEVATORS = 1;
    public const int MIN_FLOORS = 2;
    public const int MAX_ELEVATORS = 3;
    public const int MAX_FLOORS = 6;

    public static event Action<int> FloorsAmountChanged = delegate(int val) {  }; 
    public static event Action<int> ElevatorsAmountChanged = delegate(int val) {  }; 
    public static event Action Restart = delegate {  };

    private void Awake()
    {
        uiController = FindObjectOfType<UIController>(); //laziness, this should be referenced in some container object, or DiContainer

        uiController.ElevatorsAmountInput.onEndEdit.AddListener(OnElevatorsCountChanged);
        uiController.FloorsAmountInput.onEndEdit.AddListener(OnFloorsCountChanged);
        uiController.RestartBtn.onClick.AddListener(OnRestartClicked);
        uiController.ExitBtn.onClick.AddListener(OnExitClicked);
    }

    private void OnDestroy()
    {
        uiController.ElevatorsAmountInput.onEndEdit.RemoveAllListeners();
        uiController.FloorsAmountInput.onEndEdit.RemoveAllListeners();
        uiController.RestartBtn.onClick.RemoveAllListeners();
        uiController.ExitBtn.onClick.RemoveAllListeners();
    }

    private void Start()
    {
        SetDefaults();
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                uiController.SetActive(false);
                Time.timeScale = defaultTimeScale;
                isPaused = false;
            }
            else
            {
                uiController.SetActive(true);
                Time.timeScale = 0;
                isPaused = true;
            }
        }
    }

    private void Pause(bool activate)
    {
        if (activate)
        {
            uiController.SetActive(true);
            Time.timeScale = 0;
            isPaused = false;
        }
        else
        {
            uiController.SetActive(false);
            Time.timeScale = defaultTimeScale;
            isPaused = false;
        }
    }

    private void SetDefaults()
    {
        defaultTimeScale = Time.timeScale;

        floorsAmount = DEFAULT_FLOORS_AMOUNT;
        elevatorsAmount = DEFAULT_ELEVATORS_AMOUNT;

        ElevatorsAmountChanged.Invoke(elevatorsAmount);
        FloorsAmountChanged.Invoke(floorsAmount);
    }

    public void SetNumElevators(int elevatorsAmount) //for tests
    {
        this.elevatorsAmount = elevatorsAmount;
    }

    private void OnElevatorsCountChanged(string value)
    {
        var amount = CheckInput(value);

        if (amount == -1)
        {
            return;
        }

        elevatorsAmount = amount;
        ElevatorsAmountChanged.Invoke(amount);
    }

    private void OnFloorsCountChanged(string value)
    {
        var amount = CheckInput(value);

        if (amount == -1)
        {
            return;
        }

        floorsAmount = amount;
        FloorsAmountChanged.Invoke(amount);
    }

    private static int CheckInput(string str)
    {
        if (!int.TryParse(str, out var num))
        {
            Debug.LogError($"invalid input {str}");
            return -1;
        }

        return num;
    }

    public void OnRestartClicked()
    {
        var goodToGo = true;

        if (elevatorsAmount < 1 || elevatorsAmount > MAX_ELEVATORS)
        {
            Debug.LogError($"elevators amount should be 1 .. {MAX_ELEVATORS}");
            goodToGo = false;
        }

        if (floorsAmount < 1 || floorsAmount > MAX_FLOORS)
        {
            Debug.LogError($"floors amount should be 1 .. {MAX_FLOORS}");
            goodToGo = false;
        }

        if (!goodToGo)
        {
            return;
        }

        Restart.Invoke();

        basement.Restart(elevatorsAmount, floorsAmount);
        isPlaying = true;
        
        Pause(false);
    }

    //todo: other fancy stuff here
}
