using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// data about where an item should be placed 
/// </summary>
[Serializable]
public struct PlacementPose
{
    public bool IsValid;
    public Transform Parent;
    public Vector3 Position;
    public Quaternion Rotation;
}
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

    public Player Player;
    private float playerCamYawOffset = 0f;
    void Awake()
    {
        Player = GetComponent<Player>();

    }
    void Start()
    {
        PlacingRange = Player.Interaction.InteractRange;
    }
    void Update()
    {
        if (!isHolding) return;

        placementInfo.WorldRaycastValid = Physics.Raycast(CamRoot.position, CamRoot.forward, out placementInfo.WorldRaycastHit, PlacingRange, PlacingMask, QueryTriggerInteraction.Ignore);
        // placementInfo.SnapRaycastValid = Physics.RaycastAll(CamRoot.position, CamRoot.forward, out placementInfo.SnapRaycastHits, PlacingRange, SnapMask, QueryTriggerInteraction.Collide);
        placementInfo.SnapRaycastHits = Physics.RaycastAll(CamRoot.position, CamRoot.forward, PlacingRange, SnapMask, QueryTriggerInteraction.Collide);
        placementInfo.SnapRaycastValid = placementInfo.SnapRaycastHits.Length > 0;

        UpdatePlacementPreview(placementPreview, placementInfo, ItemInHand);
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
        if (!isHolding) return;

        if (ItemInHand.OnHeldPressedInteract(this, context))
        {
            return;
        }

        //place 
        if (context.Type == InteractType.Primary)
        {
            if (TryGetPlacementPose(ItemInHand, placementInfo, out PlacementPose pose))
            {
                TryPlaceFromPose(pose, placementInfo);
            }
        }
    }

    void FinishHold()
    {
        ItemInHand.OnAfterHeld(this);
        OnItemHeld?.Invoke(ItemInHand);

    }
    /// <summary>
    /// Called after placing a held item
    /// </summary>
    void FinishPlace()
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
        ItemInHand.OnAfterPlace(this);

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

        //   compute angle offset of the holdable
        float playerYaw = CamRoot.eulerAngles.y;
        float itemYaw = target.transform.eulerAngles.y;
        playerCamYawOffset = Mathf.DeltaAngle(playerYaw, itemYaw);



        // parent and position to hand. and disable it
        target.transform.SetParent(HoldSpot, worldPositionStays: true);

        target.transform.localPosition = Vector3.zero;
        target.transform.localRotation = Quaternion.identity;

        // target.transform.localPosition = target.HoldLocalPosition;
        // target.transform.localRotation = Quaternion.Euler(target.HoldLocalEuler);

        // target.transform.rotation = HoldSpot.rotation;
        target.gameObject.SetActive(false);

        // generate a visual root clone and parent to hand
        handVisual = target.GetParentChildLinkedVisualRootClone();
        handVisual.transform.SetParent(HoldSpot, worldPositionStays: true);

        // handVisual.transform.localRotation = Quaternion.identity;
        // handVisual.transform.localPosition = target.HoldLocalPosition;
        // handVisual.transform.localRotation = Quaternion.Euler(target.HoldLocalEuler);

        handVisual.transform.localPosition = Vector3.zero;
        handVisual.transform.localRotation = Quaternion.identity;



        ItemInHand = target;

        FinishHold();
        return true;

    }


    // /// <summary>
    // /// Tries to place the held item if it can.
    // /// </summary>
    // /// <param name="pos"></param>
    // public bool TryPlaceOnSurface(Transform surface, Vector3 pos, Quaternion rot)
    // {
    //     if (!isHolding) return false;
    //     Debug.Log("[PlayerItemHolder]: Trying to place on surface.");


    //     //place
    //     ItemInHand.transform.SetParent(surface, worldPositionStays: true);
    //     ItemInHand.transform.position = pos;
    //     ItemInHand.transform.rotation = rot;

    //     FinishPlace();
    //     return true;
    // }


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


    bool TryGetPlacementPose(Holdable heldHoldable, PlacementInfo info, out PlacementPose pose)
    {
        pose = default;

        if (heldHoldable == null)
            return false;

        // Custom placement has highest priority
        if (heldHoldable.TryGetCustomPlacementPreview(this, info, out Vector3 customPos, out Quaternion customRot, out bool show))
        {
            if (!show)
                return false;

            pose.IsValid = true;
            pose.Parent = null;
            pose.Position = customPos;
            pose.Rotation = customRot;
            return true;
        }

        // Snap placement
        if (heldHoldable.TryGetComponent<Snapper>(out var snapper) &&
            snapper.TryGetPreviewPose(info, out Vector3 snapPos, out Quaternion snapRot))
        {
            pose.IsValid = true;
            pose.Parent = null;
            pose.Position = snapPos;
            pose.Rotation = snapRot;
            return true;
        }

        // World placement
        if (info.TryGetWorldPlacementInfo(out Transform surface, out Vector3 worldPos, out Quaternion _))
        {
            float playerYaw = CamRoot.eulerAngles.y;
            float totalYaw = playerYaw + playerCamYawOffset + info.WorldPlacementYaw;

            pose.IsValid = true;
            pose.Parent = surface;
            pose.Position = worldPos;
            pose.Rotation = Quaternion.Euler(0f, totalYaw, 0f);
            return true;
        }

        return false;
    }


    bool TryPlaceFromPose(PlacementPose pose, PlacementInfo placementInfo)
    {
        if (!isHolding || !pose.IsValid)
            return false;

        ItemInHand.transform.SetParent(pose.Parent, worldPositionStays: true);
        ItemInHand.transform.position = pose.Position;
        ItemInHand.transform.rotation = pose.Rotation;

        // handle snapper
        if (ItemInHand.TryGetComponent<Snapper>(out var snapper))
        {
            if (snapper.TryPlaceFromPlacementInfo(placementInfo))
            {
                FinishPlace();
                return true;
            }
        }




        FinishPlace();
        return true;
    }




    void UpdatePlacementPreview(PlacementPreview preview, PlacementInfo placementInfo, Holdable heldHoldable)
    {
        if (TryGetPlacementPose(heldHoldable, placementInfo, out PlacementPose pose))
        {
            preview.IsShowing = true;
            preview.Position = pose.Position;
            preview.Rotation = pose.Rotation;
        }
        else
        {
            preview.IsShowing = false;
        }

        if (preview.IsShowing)
        {
            if (preview.PreviewObject == null)
            {
                preview.PreviewObject = heldHoldable.GetParentChildLinkedVisualRootClone();

                preview.PreviewObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                foreach (Transform t in preview.PreviewObject.GetComponentsInChildren<Transform>())
                {
                    t.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
                }
            }

            preview.PreviewObject.transform.position = preview.Position;
            preview.PreviewObject.transform.rotation = preview.Rotation;
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