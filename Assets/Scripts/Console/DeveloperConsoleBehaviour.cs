using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class StartupConsoleCommand
{
    [InlineEditor]
    public ConsoleCommand Command;

    public string Arguments;

    public string BuildInput(string prefix)
    {
        return string.IsNullOrWhiteSpace(Arguments)
            ? $"{prefix}{Command.CommandWord}"
            : $"{prefix}{Command.CommandWord} {Arguments}";
    }
}

public class DeveloperConsoleBehaviour : MonoBehaviour
{
    [SerializeField] private string prefix = string.Empty;
    [SerializeField, InlineEditor] private ConsoleCommand[] commands = Array.Empty<ConsoleCommand>();
    [SerializeField] private StartupConsoleCommand[] startCommands = Array.Empty<StartupConsoleCommand>();

    [Header("UI")]
    [SerializeField] private GameObject uiCanvas;
    [SerializeField] private TMP_InputField inputField;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private InputActionReference toggleConsole;
    [SerializeField] private string playerModeActionMapName = "Player";

    private float pausedTimeScale;
    private static DeveloperConsoleBehaviour instance;

    private DeveloperConsole developerConsole;
    private DeveloperConsole DeveloperConsole =>
        developerConsole ??= new DeveloperConsole(prefix, commands);

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

    private void Start()
    {
        foreach (var cmd in startCommands)
        {
            DeveloperConsole.ProcessCommand(cmd.BuildInput(prefix));
        }
    }

    private void Update()
    {
        if (toggleConsole.action.WasPressedThisFrame())
            ToggleConsole();
    }

    public void ToggleConsole()
    {
        if (uiCanvas.activeSelf)
            HideConsole();
        else
            ShowConsole();
    }

    private void HideConsole()
    {
        Time.timeScale = pausedTimeScale;
        inputActionAsset.FindActionMap(playerModeActionMapName).Enable();
        uiCanvas.SetActive(false);
    }

    private void ShowConsole()
    {
        pausedTimeScale = Time.timeScale;
        Time.timeScale = 0;

        uiCanvas.SetActive(true);
        inputActionAsset.FindActionMap(playerModeActionMapName).Disable();
        inputField.ActivateInputField();
    }

    public void ProcessCommand(string inputValue)
    {
        DeveloperConsole.ProcessCommand(inputValue);
        inputField.text = string.Empty;
        HideConsole();
    }
}