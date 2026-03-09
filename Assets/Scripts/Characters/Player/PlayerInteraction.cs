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
    public PlayerController Controller;
    public Transform CamRoot;
    public InputActionReference PrimaryInteract;
    public InputActionReference SecondaryInteract;
    public LayerMask InteractionMask;

    public event Action<InteractableBase> OnInteractableChanged;

    private Player player;
    private InteractableBase hoveredInteractable;
    private InteractionHit interactionHit = new InteractionHit();




    void Awake()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        // bool hitValid;
        // RaycastHit hit;
        if (Physics.Raycast(CamRoot.position, CamRoot.forward, out interactionHit.Hit, InteractRange, InteractionMask))
        {
            if (interactionHit.Hit.collider.TryGetComponent<InteractableBase>(out var currentInteractable))
            {
                // TryHighlightHoverInteractable(false);
                if (currentInteractable != hoveredInteractable)
                {

                    InteractableChanged(hoveredInteractable, currentInteractable);

                }
                hoveredInteractable = currentInteractable;
                // TryHighlightHoverInteractable(true);

            }

        }
        else
        {
            // not looking at interactable 
            TryHighlightHoverInteractable(false);
            if (hoveredInteractable != null)
            {
                InteractableChanged(hoveredInteractable, null);
                hoveredInteractable = null;
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

    void InteractableChanged(InteractableBase oldInteractable, InteractableBase newInteractable)
    {
        oldInteractable?.OnHoverExit();
        newInteractable?.OnHoverEnter();

        oldInteractable?.SetOutline(false);
        newInteractable?.SetOutline(true);

        OnInteractableChanged?.Invoke(newInteractable);

    }

    void HandleInteraction(InteractType type)
    {
        InteractionContext context = new InteractionContext(type, GetComponent<Player>());

        // based on some player subsystem
        // if (Controller.IsCameraContrained == true)
        // {
        //     Controller.UnConstrainCamera();
        //     return;
        // }


        if (Controller.CurrentControlMode == PlayerMode.InPopup)
        {
            Controller.CloseCurrentPopup();
            return;
        }

        if (Controller.CurrentControlMode == PlayerMode.BodyConstrained)
        {
            Controller.UnconstrainBody();
            // Controller.SetPlayerMode(PlayerMode.FullGameplay);
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
        }

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

        // if (hoveredInteractable != null)
        // {
        //     Handles.Label(hitPoint, "Interactable hit point");
        //     Gizmos.DrawWireSphere(hitPoint, .2f);

        //     Handles.Label(transform.position, "Current interactable: " + hoveredInteractable.transform.name);
        // }

    }
#endif
}