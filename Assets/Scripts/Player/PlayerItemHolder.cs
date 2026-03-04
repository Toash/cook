using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
    [HideInInspector]
    public float PlacingRange; // same as interact range



    public bool isHolding
    {
        get
        {
            return itemInHand != null;
        }
    }

    [ShowInInspector, ReadOnly]
    private Holdable itemInHand;
    [ShowInInspector, ReadOnly]
    private PlacementRaycastInfo placementRaycastInfo = new();
    [ShowInInspector, ReadOnly]
    private PlacementPreview placementPreview = new();

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

        placementRaycastInfo.WorldRaycastValid = Physics.Raycast(CamRoot.position, CamRoot.forward, out placementRaycastInfo.WorldRaycastHit, PlacingRange, PlacingMask, QueryTriggerInteraction.Ignore);
        placementRaycastInfo.SnapRaycastValid = Physics.Raycast(CamRoot.position, CamRoot.forward, out placementRaycastInfo.SnapRaycastHit, PlacingRange, SnapMask, QueryTriggerInteraction.Collide);

        HandlePlacementPreview(placementPreview, placementRaycastInfo, itemInHand);
    }

    public void OnInteractAndHolding(InteractionContext context)
    {
        if (context.Type == InteractType.Primary)
        {
            // try place via snapping
            if (itemInHand.TryGetComponent<Snapper>(out var _))
            {
                if (placementRaycastInfo.SnapRaycastValid)
                {
                    if (TryPlaceOnSnapper(placementRaycastInfo))
                    {
                        return;
                    }
                }
            }

            if (placementRaycastInfo.WorldRaycastValid)
            {
                // or, place on surface. 
                TryPlaceOnSurface(placementRaycastInfo.WorldRaycastHit.point);
            }
        }

    }
    void HandlePlacementPreview(PlacementPreview preview, PlacementRaycastInfo placementInfo, Holdable heldHoldable)
    {
        // snap point
        if (placementInfo.TryGetSnapArea(out var snapPoint))
        {
            preview.IsShowing = true;
            preview.Position = snapPoint.transform.position;
            preview.Rotation = snapPoint.transform.rotation;
        }
        // world surface 
        else if (placementInfo.TryGetWorldPlacementPos(out var pos))
        {
            preview.IsShowing = true;
            preview.Position = pos;
            preview.Rotation = Quaternion.identity;
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
                preview.PreviewObject = Instantiate(heldHoldable.GetVisualRoot());
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

    /// <summary>
    /// Callback for when a holdable is placed
    /// </summary>
    void OnPlace()
    {
        DropHoldable(itemInHand);
        TryDeletePreview(placementPreview);
    }
    public void TryHold(Holdable target)
    {
        if (isHolding) return;

        // set parent of the target to the hand socket. keep world position
        target.transform.SetParent(HoldSpot, worldPositionStays: true);

        // lerp local pos to Vector3 zero
        // lerpm rotation to identity

        target.transform.localPosition = Vector3.zero;
        target.transform.rotation = HoldSpot.rotation;


        itemInHand = target;
    }

    void DropHoldable(Holdable holdable)
    {
        holdable.SetNotHolding();
        itemInHand = null;
    }


    /// <summary>
    /// Tries to place the held item if it can.
    /// </summary>
    /// <param name="pos"></param>
    public bool TryPlaceOnSurface(Vector3 pos)
    {
        if (!isHolding) return false;
        Debug.Log("[PlayerItemHolder]: Trying to place on surface.");


        //place
        itemInHand.transform.SetParent(null);
        itemInHand.transform.position = pos;
        itemInHand.transform.rotation = Quaternion.identity;

        OnPlace();
        return true;
    }

    public bool TryPlaceOnSnapper(PlacementRaycastInfo placementRaycastInfo)
    {
        if (!isHolding) return false;

        Debug.Log("[PlayerItemHolder]: Trying to place on snap area.");

        if (itemInHand.TryGetComponent<Snapper>(out Snapper heldSnapper))
        {
            if (placementRaycastInfo.TryGetSnapArea(out SnapArea otherSnapPoint))
            {
                Snapper otherSnapper = otherSnapPoint.ParentSnapper;

                if (heldSnapper.CanSnap(otherSnapper))
                {
                    //place
                    heldSnapper.SnapToArea(placementRaycastInfo, otherSnapper, otherSnapPoint);
                    OnPlace();
                    return true;
                }
            }


        }
        return false;
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
            if (placementRaycastInfo.WorldRaycastValid)
            {
                style.normal.textColor = Color.green;
                Gizmos.DrawWireSphere(placementRaycastInfo.WorldRaycastHit.point, .3f);

                Handles.Label(placementRaycastInfo.WorldRaycastHit.point, placementRaycastInfo.WorldRaycastHit.collider.name);
            }

        }
    }

#endif









}