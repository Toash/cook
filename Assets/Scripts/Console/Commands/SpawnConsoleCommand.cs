using UnityEngine;

[CreateAssetMenu(menuName = "Console Commands/Spawn Command")]
public class SpawnConsoleCommand : ConsoleCommand
{
    private enum SpawnMode
    {
        Front,
        Hand,
        Hit
    }

    public override bool Process(string[] args)
    {
        if (HoldableSpawnDatabase.Instance == null)
        {
            Debug.LogWarning("Spawn failed: no HoldableSpawnDatabase found in scene.");
            return false;
        }

        if (args == null || args.Length == 0)
        {
            Debug.LogWarning("Usage: spawn <itemName|random> [front|hand|hit]");
            return false;
        }

        string itemArg = args[0].ToLower();

        HoldableData data = itemArg == "random"
            ? HoldableSpawnDatabase.Instance.GetRandom()
            : HoldableSpawnDatabase.Instance.Get(itemArg);

        if (data == null)
        {
            Debug.LogWarning($"Spawn failed: no HoldableData found for '{itemArg}'.");
            return false;
        }

        if (data.HoldablePrefab == null)
        {
            Debug.LogWarning($"Spawn failed: HoldableData '{data.name}' has no prefab.");
            return false;
        }

        SpawnMode mode = SpawnMode.Front;

        if (args.Length >= 2)
        {
            string modeArg = args[1].ToLower();

            if (!TryParseMode(modeArg, out mode))
            {
                Debug.LogWarning($"Unknown spawn mode: {modeArg}");
                return false;
            }
        }

        Spawn(data, mode);
        return true;
    }

    private void Spawn(HoldableData data, SpawnMode mode)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Spawn failed: no main camera found.");
            return;
        }

        Player player = Object.FindFirstObjectByType<Player>();

        Vector3 pos = GetSpawnPosition(cam, mode);
        Quaternion rot = Quaternion.identity;

        GameObject spawned = Instantiate(data.HoldablePrefab.gameObject, pos, rot);

        if (mode == SpawnMode.Hand)
        {
            Holdable holdable = spawned.GetComponent<Holdable>();

            if (holdable != null && player != null && player.ItemHolder != null)
            {
                player.ItemHolder.TryHold(holdable);
            }
            else
            {
                Debug.LogWarning("Spawned item in hand mode, but could not place it in player hand.");
            }
        }

        Debug.Log($"Spawned {data.Name} ({mode})");
    }

    private Vector3 GetSpawnPosition(Camera cam, SpawnMode mode)
    {
        switch (mode)
        {
            case SpawnMode.Hand:
                return Vector3.zero;

            case SpawnMode.Hit:
                {
                    Ray ray = new Ray(cam.transform.position, cam.transform.forward);
                    if (Physics.Raycast(ray, out RaycastHit hit, 10f))
                    {
                        return hit.point + hit.normal * 0.15f;
                    }

                    return cam.transform.position + cam.transform.forward * 2f;
                }

            case SpawnMode.Front:
            default:
                return cam.transform.position + cam.transform.forward * 2f;
        }
    }

    private bool TryParseMode(string arg, out SpawnMode mode)
    {
        switch (arg)
        {
            case "front":
                mode = SpawnMode.Front;
                return true;

            case "hand":
                mode = SpawnMode.Hand;
                return true;

            case "hit":
                mode = SpawnMode.Hit;
                return true;

            default:
                mode = SpawnMode.Front;
                return false;
        }
    }
}