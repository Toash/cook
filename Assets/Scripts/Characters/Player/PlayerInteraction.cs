using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;



public class InteractionHit
{
    public bool HitValid;
    public RaycastHit Hit;
}
[RequireComponent(typeof(Player))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Stats")]
    public float InteractRange = 3;

    [Header("References")]
    public PlayerItemHolder ItemHolder;
    public PlayerController Controller;
    public Transform CamRoot;
    public InputActionReference PrimaryInteract;
    public InputActionReference SecondaryInteract;
    public LayerMask InteractionMask;

    /// <summary>
    /// Called when an interactable gets changed. can be used for updating the ui. <br/>
    /// doesnt necessarily correspond to the current interactable.
    /// </summary>
    public event Action<IInteractable> OnInteractableChanged;
    public event Action<InteractableBase> OnHoveredInteractableChanged;

    public event Action<InteractableBase> OnHoveredInteractableInteracted;


    private Player player;
    private InteractableBase hoveredInteractable;
    private InteractionHit interactionHit = new InteractionHit();




    void Awake()
    {
        player = GetComponent<Player>();
    }

    void OnEnable()
    {
        Controller.BodyConstrained += InteractableChanged;
        ItemHolder.OnItemHeld += InteractableChanged;


    }
    void OnDisable()
    {
        Controller.BodyConstrained -= InteractableChanged;
        ItemHolder.OnItemHeld -= InteractableChanged;

    }


    void Update()
    {
        // bool hitValid;
        // RaycastHit hit;
        if (Physics.Raycast(CamRoot.position, CamRoot.forward, out interactionHit.Hit, InteractRange, InteractionMask))
        {
            if (interactionHit.Hit.collider.TryGetComponent<InteractableBase>(out var currentInteractable))
            {
                // hoveredInteractable = currentInteractable;

                // TryHighlightHoverInteractable(false);
                if (currentInteractable != hoveredInteractable)
                {

                    HoverInteractableChanged(hoveredInteractable, currentInteractable);

                }
                // TryHighlightHoverInteractable(true);

            }

        }
        else
        {
            // not looking at interactable 
            TryHighlightHoverInteractable(false);
            if (hoveredInteractable != null)
            {
                // hoveredInteractable = null;
                HoverInteractableChanged(hoveredInteractable, null);
            }
        }

        if (PrimaryInteract.action.WasPressedThisFrame())
        {
            HandleInteraction(InteractType.Primary);
        }
        if (SecondaryInteract.action.WasPressedThisFrame())
        {
            HandleInteraction(InteractType.Secondary);
        }

    }

    /// <summary>
    /// The source of truth for whatever the player will interact with.
    /// </summary>
    /// <returns></returns>
    public IInteractable GetCurrentInteractable()
    {

        // return interactable from the constrainer.
        if (Controller.CurrentControlMode == PlayerMode.BodyConstrained)
        {
            return Controller.CurrentConstrainerInteractable;

        }

        // return interactable from player hand
        if (player.ItemHolder.isHolding)
        {
            return player.ItemHolder.ItemInHand;
        }

        //return hovered interactable
        if (hoveredInteractable != null)
        {
            return hoveredInteractable;
        }

        return null;
    }
    void InteractableChanged(IInteractable newInteractable)
    {
        Debug.Log("[PlayerInteraction]: Interactable Changed: " + newInteractable);
        OnInteractableChanged?.Invoke(newInteractable);
    }

    void HoverInteractableChanged(InteractableBase oldInteractable, InteractableBase newInteractable)
    {

        hoveredInteractable = newInteractable;

        oldInteractable?.OnHoverExit();
        newInteractable?.OnHoverEnter();

        oldInteractable?.SetOutline(false);
        newInteractable?.SetOutline(true);

        OnHoveredInteractableChanged?.Invoke(newInteractable);

        InteractableChanged(newInteractable);

    }

    void HandleInteraction(InteractType type)
    {
        InteractionContext context = new InteractionContext(type, GetComponent<Player>());


        // interactinon based on the current context
        if (Controller.CurrentControlMode == PlayerMode.InPopup)
        {
            Controller.CloseCurrentPopup();
            return;
        }
        if (Controller.CurrentControlMode == PlayerMode.BodyConstrained)
        {
            Controller.OnInteractAndConstraint(context);
            // Controller.UnconstrainBody();
            // Controller.CurrentConstrainerInteractable.Interact(context);
            return;
        }

        if (player.ItemHolder.isHolding)
        {
            player.ItemHolder.OnInteractAndHolding(context);
            return;
        }

        // based on raycast
        if (hoveredInteractable != null)
        {
            hoveredInteractable.BaseInteract(context);
            OnHoveredInteractableInteracted?.Invoke(hoveredInteractable);
        }

        // IInteractable CurrentInteractable = GetCurrentInteractable();
        // if (CurrentInteractable != null)
        // {
        //     CurrentInteractable.Interact(context);
        //     if (CurrentInteractable is InteractableBase interactableBase)
        //     {
        //         OnHoveredInteractableInteracted?.Invoke(interactableBase);
        //     }
        // }

    }


    void TryHighlightHoverInteractable(bool highlight)
    {
        if (hoveredInteractable == null) return;
        hoveredInteractable.SetOutline(highlight);
    }


#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(CamRoot.position, CamRoot.forward * InteractRange);

        style.normal.textColor = Color.blue;
        Gizmos.DrawWireSphere(CamRoot.position, InteractRange);
        Handles.Label(CamRoot.position + CamRoot.forward * InteractRange, "Interact Range");


    }
#endif
}