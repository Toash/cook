using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// Container that holds multiple holdables.
/// </summary>
[RequireComponent(typeof(Holdable))]
public class HoldableContainer : MonoBehaviour
{

    [ReadOnly]
    public Holdable ContainedHoldablePrefab { get; private set; }
    public int MaxAmount = 5;
    private int currentAmount;


    private Holdable holdable;
    private Snapper snapper;

    void Awake()
    {
        holdable = GetComponent<Holdable>();
    }
    void Start()
    {
        if (ContainedHoldablePrefab == null)
        {
            Debug.Log("[HoldableContainer]: contained holdable prefab is null!");
        }
        UpdateTooltip();
    }

    void OnEnable()
    {
        holdable.OnInteract += OnInteract;
    }
    void OnDisable()
    {
        holdable.OnInteract -= OnInteract;
    }

    public void Init(Holdable holdable, int startingAmount)
    {
        ContainedHoldablePrefab = holdable;
        currentAmount = startingAmount;
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

        Holdable holdable = Instantiate(ContainedHoldablePrefab);
        context.Player.ItemHolder.TryHold(holdable);

        currentAmount--;

        UpdateTooltip();
    }

    void UpdateTooltip()
    {
        string message = "";
        if (ContainedHoldablePrefab.TryGetComponent<Ingredient>(out var ingredient))
        {
            message += ingredient.Data.Name + "s ";
        }
        message += currentAmount + "/" + MaxAmount;

        holdable.HoverTooltip = message;
    }





}