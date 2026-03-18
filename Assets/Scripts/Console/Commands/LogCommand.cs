using UnityEngine;

[CreateAssetMenu(fileName = "New Log Command", menuName = "Console Commands/Log Command")]
public class LogCommand : ConsoleCommand
{
    [SerializeField] private string defaultLogText = "Log message";
    public override bool Process(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.Log(defaultLogText);
            return true;
        }
        string logText = string.Join(" ", args);

        Debug.Log(logText);

        return true;
    }
}