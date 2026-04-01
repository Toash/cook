using UnityEngine;

public class HoldableContainerManager : MonoBehaviour
{
    public static HoldableContainerManager I { get; private set; }

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
    HoldableData containedItemData,
    int amount,
    Vector3 position,
    Quaternion rotation
    )
    {
        if (containedItemData == null)
        {
            Debug.LogError("[HoldableContainerManager]: containedItemData is null.", this);
            return null;
        }

        if (amount <= 0)
        {
            Debug.LogWarning("[HoldableContainerManager]: amount must be > 0.", this);
            return null;
        }

        HoldableContainer prefabToSpawn = containedItemData.ContainerPrefab;

        if (prefabToSpawn == null)
        {
            Debug.LogError($"[HoldableContainerManager]: no container prefab found for '{containedItemData.Name}'.", this);
            return null;
        }

        HoldableContainer container = Instantiate(prefabToSpawn, position, rotation);
        container.Container.Init(containedItemData, amount, amount);
        return container;
    }
}