using UnityEngine;

public class HoldableContainerManager : MonoBehaviour
{
    public static HoldableContainerManager I { get; private set; }

    [SerializeField] private HoldableContainer containerPrefab;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    public HoldableContainer SpawnContainer(
        HoldableData item,
        int amount,
        Vector3 position,
        Quaternion rotation)
    {
        if (containerPrefab == null)
        {
            Debug.LogError("[HoldableContainerManager]: containerPrefab is null.", this);
            return null;
        }

        if (item == null)
        {
            Debug.LogError("[HoldableContainerManager]: item is null.", this);
            return null;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("[HoldableContainerManager]: amount must be > 0.");
            return null;
        }

        HoldableContainer container = Instantiate(containerPrefab, position, rotation);

        // currentAmount = amount, maxAmount = amount (fully filled container)
        container.Init(item, amount, amount);

        return container;
    }
}