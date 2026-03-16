using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
/// <summary>
/// Container that holds multiple holdables.
/// </summary>
[RequireComponent(typeof(Holdable))]
public class HoldableContainer : MonoBehaviour
{

    [Tooltip("What this container contains.")]
    public HoldableData ContainedItem;
    public int MaxAmount = 5;

    private int currentAmount;
    private Holdable thisHoldable;
    private Holdable containedHoldable;
    private bool initialized = false;

    void Awake()
    {
        thisHoldable = GetComponent<Holdable>();
    }
    void Start()
    {
        if (containedHoldable == null)
        {
            Debug.Log("[HoldableContainer]: contained holdable prefab is null!");
        }
        Init(ContainedItem, MaxAmount);
        UpdateTooltip();

    }

    void OnEnable()
    {
        thisHoldable.OnInteract += OnInteract;
    }
    void OnDisable()
    {
        thisHoldable.OnInteract -= OnInteract;
    }

    public void Init(HoldableData itemData, int startingAmount)
    {
        if (initialized) return;
        if (itemData == null) return;

        ContainedItem = itemData;
        containedHoldable = itemData.HoldablePrefab;

        MaxAmount = startingAmount;
        currentAmount = startingAmount;

        initialized = true;
    }



    void OnInteract(InteractionContext context)
    {
        if (context.Type == InteractType.Secondary)
        {
            Debug.Log("Secondary");
            Pickup(context);
        }
    }


    void Pickup(InteractionContext context)
    {
        if (currentAmount <= 0) return;

        Holdable holdable = Instantiate(containedHoldable);
        context.Player.ItemHolder.TryHold(holdable);

        currentAmount--;

        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        string message = "";
        if (containedHoldable.TryGetComponent<Ingredient>(out var ingredient))
        {
            message += ingredient.Data.Name + "s ";
            message += currentAmount + "/" + MaxAmount;
        }
        else if (ContainedItem != null)
        {
            message += ContainedItem.Name + "s ";
        }

        var data = new HoverTooltipData(transform, message);
        thisHoldable.HoverTooltipData = data;
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (ContainedItem != null)
        {
            Handles.Label(transform.position, ContainedItem.Name);
        }

    }
#endif



}