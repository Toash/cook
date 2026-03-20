using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// System for holding items.  that can be used.
/// </summary>
[RequireComponent(typeof(Player))]
public class PlayerItemHolder : MonoBehaviour
{

    public Transform CamRoot;
    [Header("References"), Tooltip("Where the item will go when picked up.")]
    public Transform HoldSpot;

    [Tooltip("Mask used for placing things.")]
    public LayerMask PlacingMask;
    [Tooltip("Mask used for determining where snap points are.")]
    public LayerMask SnapMask;


    [Header("Input")]
    public InputActionReference RotatePlacementAction;
    public InputActionReference LookAction;
    public float RotateSpeed = 5f; // how fast the item rotates when rotating placement preview.


    [HideInInspector]
    public float PlacingRange; // same as interact range



    public bool isHolding
    {
        get
        {
            return ItemInHand != null;
        }
    }

    [ShowInInspector, ReadOnly]
    public Holdable ItemInHand { get; private set; }


    public event Action<IInteractable> OnItemHeld;


    [ShowInInspector, ReadOnly]
    private PlacementInfo placementInfo = new();
    [ShowInInspector, ReadOnly]
    private PlacementPreview placementPreview = new();
    private GameObject handVisual = null;

    private Player player;
    void Awake()
    {
        player = GetComponent<Player>();

    }
    void Start()
    {
        PlacingRange = player.Interaction.InteractRange;
    }
    void Update()
    {
        if (!isHolding) return;

        placementInfo.WorldRaycastValid = Physics.Raycast(CamRoot.position, CamRoot.forward, out placementInfo.WorldRaycastHit, PlacingRange, PlacingMask, QueryTriggerInteraction.Ignore);
        // placementInfo.SnapRaycastValid = Physics.RaycastAll(CamRoot.position, CamRoot.forward, out placementInfo.SnapRaycastHits, PlacingRange, SnapMask, QueryTriggerInteraction.Collide);
        placementInfo.SnapRaycastHits = Physics.RaycastAll(CamRoot.position, CamRoot.forward, PlacingRange, SnapMask, QueryTriggerInteraction.Collide);
        placementInfo.SnapRaycastValid = placementInfo.SnapRaycastHits.Length > 0;

        HandlePlacementPreview(placementPreview, placementInfo, ItemInHand);
        if (isHolding)
        {
            if (RotatePlacementAction.action.IsPressed())
            {

                Vector2 delta = LookAction.action.ReadValue<Vector2>();
                placementInfo.WorldPlacementYaw += delta.x * RotateSpeed;
            }
        }
    }

    public void OnInteractAndHolding(InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            // try place via snapping
            if (ItemInHand.TryGetComponent<Snapper>(out var _))
            {
                if (placementInfo.SnapRaycastValid)
                {
                    if (TryPlaceOnSnapper(placementInfo))
                    {
                        return;
                    }
                }
            }

            if (placementInfo.TryGetWorldPlacementInfo(out Transform trans, out Vector3 pos, out Quaternion rot))
            {
                TryPlaceOnSurface(trans, pos, rot);
            }
        }

    }
    void OnAfterHeld()
    {
        ItemInHand.OnAfterHeld();
        OnItemHeld?.Invoke(ItemInHand);

    }
    void OnAfterPlace()
    {
        ItemInHand.gameObject.SetActive(true);
        ItemInHand.SetNotHolding();
        TryDeletePreview(placementPreview);
        placementInfo.WorldPlacementYaw = 0;
        if (handVisual != null)
        {
            Destroy(handVisual);
            handVisual = null;
        }
        ItemInHand.OnAfterPlace();

        ItemInHand = null;
    }
    /// <summary>
    /// Tries to hold the target holdable. returns true if successful.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool TryHold(Holdable target)
    {
        if (isHolding) return false;


        if (target.TryGetComponent<Snapper>(out var snapper))
        {
            snapper.DetachFromParent();
        }




        // parent and position to hand. and disable it
        target.transform.SetParent(HoldSpot, worldPositionStays: true);
        target.transform.localPosition = Vector3.zero;
        target.transform.rotation = HoldSpot.rotation;
        target.gameObject.SetActive(false);

        // generate a visual root clone and parent to hand
        handVisual = target.GetParentChildLinkedVisualRootClone();
        handVisual.transform.SetParent(HoldSpot, worldPositionStays: true);
        handVisual.transform.rotation = HoldSpot.rotation;

        ItemInHand = target;

        OnAfterHeld();
        return true;

    }


    /// <summary>
    /// Tries to place the held item if it can.
    /// </summary>
    /// <param name="pos"></param>
    public bool TryPlaceOnSurface(Transform surface, Vector3 pos, Quaternion rot)
    {
        if (!isHolding) return false;
        Debug.Log("[PlayerItemHolder]: Trying to place on surface.");


        //place
        ItemInHand.transform.SetParent(surface, worldPositionStays: true);
        ItemInHand.transform.position = pos;
        ItemInHand.transform.rotation = rot;

        OnAfterPlace();
        return true;
    }

    /// <summary>
    /// If the held item is a snapper
    /// </summary>
    /// <param name="placementRaycastInfo"></param>
    /// <returns></returns>
    public bool TryPlaceOnSnapper(PlacementInfo placementRaycastInfo)
    {
        if (!isHolding) return false;

        Debug.Log("[PlayerItemHolder]: Trying to place on snap area.");

        if (ItemInHand.TryGetComponent<Snapper>(out Snapper heldSnapper))
        {
            if (placementRaycastInfo.TryGetFirstValidSnapArea(heldSnapper, out SnapArea otherSnapPoint, out RaycastHit validHit))
            {
                Snapper otherSnapper = otherSnapPoint.ParentSnapper;

                //place
                if (heldSnapper.TrySnapToArea(placementRaycastInfo, otherSnapper, otherSnapPoint))
                {
                    OnAfterPlace();
                    return true;
                }

            }


        }
        return false;
    }

    /// <summary>
    /// Deletes the current held item if there was one.
    /// </summary>
    /// <returns></returns>
    public bool TryDeleteHeldItem()
    {
        if (!isHolding) return false;

        Debug.Log("[PlayerItemHolder]: Deleting held item.");

        var item = ItemInHand;

        // clean up preview and visuals
        if (handVisual != null)
        {
            Destroy(handVisual);
            handVisual = null;
        }

        TryDeletePreview(placementPreview);
        placementInfo.WorldPlacementYaw = 0;

        // detach from any systems first
        // if (item.TryGetComponent<Snapper>(out var snapper))
        // {
        //     snapper.DetachFromParent();
        // }

        item.SetNotHolding();

        Destroy(item.gameObject);
        ItemInHand = null;

        return true;
    }
    void HandlePlacementPreview(PlacementPreview preview, PlacementInfo placementInfo, Holdable heldHoldable)
    {
        // if we have a snapper and it can snap to the snap area. preview for place on snap area.
        if (heldHoldable.TryGetComponent<Snapper>(out var snapper) && placementInfo.TryGetFirstValidSnapArea(snapper, out SnapArea snapArea, out RaycastHit validHit))
        {
            preview.IsShowing = true;

            preview.Position = snapArea.GetSnapPoint(placementInfo);
            preview.Rotation = snapArea.GetSnapRotation(placementInfo);
        }
        // preview for place on world.
        else if (placementInfo.TryGetWorldPlacementInfo(out var _, out Vector3 worldPos, out Quaternion worldRot))
        {
            preview.IsShowing = true;
            preview.Position = worldPos;
            preview.Rotation = worldRot;
        }
        else
        {
            // disable preview 
            preview.IsShowing = false;
        }


        if (preview.IsShowing)
        {
            if (preview.PreviewObject == null)
            {
                preview.PreviewObject = heldHoldable.GetParentChildLinkedVisualRootClone();
                // set layer to ignore raycast
                preview.PreviewObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                foreach (Transform t in preview.PreviewObject.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }

            if (preview.PreviewObject != null)
            {
                preview.PreviewObject.transform.position = preview.Position;
                preview.PreviewObject.transform.rotation = preview.Rotation;
            }
        }
        else
        {
            TryDeletePreview(preview);
        }
    }



    bool TryDeletePreview(PlacementPreview preview)
    {
        if (preview.PreviewObject != null)
        {
            Destroy(preview.PreviewObject);
            preview.PreviewObject = null;
            preview.IsShowing = false;
            return true;
        }
        return false;

    }

#if UNITY_EDITOR
    GUIStyle style = new();
    void OnDrawGizmos()
    {
        if (HoldSpot != null)
        {
            Gizmos.DrawSphere(HoldSpot.position, .3f);
        }

        Gizmos.color = Color.blue;
        style.normal.textColor = Color.blue;
        Gizmos.DrawWireSphere(CamRoot.position, PlacingRange);
        Handles.Label(CamRoot.position + CamRoot.forward * PlacingRange, "Placing Range");

        // draw placing hitpoint
        if (isHolding)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(CamRoot.transform.position, CamRoot.forward * PlacingRange);
            if (placementInfo.WorldRaycastValid)
            {
                style.normal.textColor = Color.green;
                Gizmos.DrawWireSphere(placementInfo.WorldRaycastHit.point, .3f);

                Handles.Label(placementInfo.WorldRaycastHit.point, placementInfo.WorldRaycastHit.collider.name);
            }

        }
    }

#endif

}