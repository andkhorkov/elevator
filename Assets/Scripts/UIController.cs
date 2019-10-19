using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private InputField elevatorsAmountInput;
    [SerializeField] private InputField floorsAmountInput;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Text lblElevatorsAmount;
    [SerializeField] private Text lblFloorsAmount;
    [SerializeField] private CanvasGroup cg;

    public bool IsOnScreen { get; private set; }

    public InputField ElevatorsAmountInput => elevatorsAmountInput;
    public InputField FloorsAmountInput => floorsAmountInput;
    public Button RestartBtn => restartBtn;
    public Button ExitBtn => exitBtn;

    private void Awake()
    {
        GameController.FloorsAmountChanged += OnFloorsAmountChanged;
        GameController.ElevatorsAmountChanged += OnElevatorsAmountChanged;
        GameController.Restart += OnRestart;
        SetLimits();
    }

    private void OnDestroy()
    {
        GameController.FloorsAmountChanged -= OnFloorsAmountChanged;
        GameController.ElevatorsAmountChanged -= OnElevatorsAmountChanged;
        GameController.Restart -= OnRestart;
    }

    private void SetLimits()
    {
        lblElevatorsAmount.text += $"({GameController.MIN_ELEVATORS}..{GameController.MAX_ELEVATORS})";
        lblFloorsAmount.text += $"({GameController.MIN_FLOORS}..{GameController.MAX_FLOORS})";
    }

    //would be nice if Unity pass an instance of a UI element along with event, then I would not need to make method for each event in this case
    private void OnElevatorsAmountChanged(int amount)
    {
        elevatorsAmountInput.text = amount.ToString();
    }

    private void OnFloorsAmountChanged(int amount)
    {
        floorsAmountInput.text = amount.ToString();
    }

    private void OnRestart()
    {
        SetActive(false);
    }

    public void SetActive(bool activate)
    {
        cg.alpha = activate ? 1 : 0;
    }
}
