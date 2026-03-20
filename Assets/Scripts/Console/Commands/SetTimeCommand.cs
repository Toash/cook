using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/Set Time")]
public class SetTimeConsoleCommand : ConsoleCommand
{
    public override bool Process(string[] args)
    {
        if (TimeManager.I == null)
        {
            Debug.LogWarning("SetTime failed: no TimeManager found.");
            return false;
        }

        if (args == null || args.Length < 2)
        {
            Debug.LogWarning("Usage: settime <hour> <minute>");
            return false;
        }

        if (!int.TryParse(args[0], out int hour) ||
            !int.TryParse(args[1], out int minute))
        {
            Debug.LogWarning("SetTime failed: invalid hour or minute.");
            return false;
        }

        TimeManager.I.SetTime(hour, minute);
        return true;
    }
}