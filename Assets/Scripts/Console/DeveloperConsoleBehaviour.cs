using TMPro;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// Root object for the developer console. handles ui and input and passes commands to the DeveloperConsole class.
/// </summary>
public class DeveloperConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];
    [SerializeField] private ConsoleCommand[] startCommands = new ConsoleCommand[0];

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas = null;
    [SerializeField] private TMP_InputField inputField = null;
    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputActionReference toggleConsole;
    [SerializeField, Tooltip("The name of the action map for player mode.")] private string playerModeActionMapName = "Player";



    private float pausedTimeScale;

    private DeveloperConsole developerConsole;
    private DeveloperConsole DeveloperConsole
    {
        get
        {
            if (developerConsole != null) { return developerConsole; }
            return developerConsole = new DeveloperConsole(prefix, commands);
        }
    }
    private static DeveloperConsoleBehaviour instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        foreach (var command in startCommands)
        {
            DeveloperConsole.ProcessCommand(prefix + command.CommandWord);
        }
    }
    void Update()
    {
        if (toggleConsole.action.WasPressedThisFrame()) { ToggleConsole(); }
    }


    public void ToggleConsole()
    {

        if (uiCanvas.activeSelf)
        {
            Time.timeScale = pausedTimeScale;

            inputActionAsset.FindActionMap(playerModeActionMapName).Enable();

            uiCanvas.SetActive(false);
        }
        else
        {
            // pause
            pausedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            uiCanvas.SetActive(true);

            inputActionAsset.FindActionMap(playerModeActionMapName).Disable();

            // focus input field
            inputField.ActivateInputField();
        }
    }

    public void ProcessCommand(string inputValue)
    {
        DeveloperConsole.ProcessCommand(inputValue);

        inputField.text = string.Empty;
    }
}