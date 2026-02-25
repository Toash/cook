using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public float InteractRange = 3;

    public PlayerController Controller;
    public PlayerGrabber Grabber;
    public Transform CamRoot;
    public InputActionReference PrimaryInteract;
    public InputActionReference SecondaryInteract;
    public LayerMask InteractionMask;

    public event Action<InteractableBase> OnInteractableChanged;

    private InteractableBase hoveredInteractable;

#if UNITY_EDITOR
    private Vector3 hitPoint;
#endif




    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(CamRoot.position, CamRoot.forward, out hit, InteractRange, InteractionMask))
        {
            if (hit.collider.TryGetComponent<InteractableBase>(out var currentInteractable))
            {
                TryHighlightHoverInteractable(false);
                if (currentInteractable != hoveredInteractable)
                {

                    InteractableChanged(hoveredInteractable, currentInteractable);

                }
                hoveredInteractable = currentInteractable;
                TryHighlightHoverInteractable(true);

#if UNITY_EDITOR
                hitPoint = hit.point;
#endif
            }

        }
        else
        {
            // aiming away from an interactable
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

        OnInteractableChanged.Invoke(newInteractable);

    }

    void HandleInteraction(InteractType type)
    {
        InteractionContext context = new InteractionContext(type, Controller, this, Grabber);
        if (Controller.IsConstrained == true)
        {
            Controller.UnConstrain();
            return;
        }

        if (Grabber.isHolding)
        {
            Grabber.OnInteractAndHolding(context);
            return;
        }

        if (hoveredInteractable != null)
        {
            hoveredInteractable.Interact(context);
        }

    }


    void TryHighlightHoverInteractable(bool highlight)
    {
        if (hoveredInteractable == null) return;
        if (hoveredInteractable.transform.TryGetComponent<Highlightable>(out Highlightable highlightable))
        {
            highlightable.SetHighlight(highlight);
        }

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(CamRoot.position, CamRoot.forward * InteractRange);

        if (hoveredInteractable != null)
        {
            Handles.Label(hitPoint, "Interactable hit point");
            Gizmos.DrawWireSphere(hitPoint, .2f);

            Handles.Label(transform.position, "Current interactable: " + hoveredInteractable.transform.name);
        }

    }
#endif
}