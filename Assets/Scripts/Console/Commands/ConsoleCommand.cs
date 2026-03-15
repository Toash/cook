using UnityEngine;

public abstract class ConsoleCommand : ScriptableObject, IConsoleCommand
{

    [SerializeField, Tooltip("The word that triggers this command")] private string commandWord = string.Empty;

    public string CommandWord => commandWord;

    public abstract bool Process(string[] args);
}