using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

/// <summary>
/// An item that can be held and placed down by the player.
/// </summary>
public class Holdable : InteractableBase
{
    [Header("Hold Pose")]
    public Vector3 HoldLocalPosition = Vector3.zero;
    public Vector3 HoldLocalEuler = Vector3.zero;

    [Tooltip("Information that is shown when the item is actually held.")]
    public List<InteractInfo> HeldInfos = new List<InteractInfo>();
    [InlineEditor]
    public HoldableData ItemData;
    public event Action<InteractionContext> OnHeld;
    public event Action<InteractionContext> OnSecondaryInteract;


    protected bool beingHeld = false;

    private AudioSourcePlayer audioSourcePlayer;

    void InitRefs()
    {
        audioSourcePlayer = GetComponent<AudioSourcePlayer>();
        if (audioSourcePlayer == null)
        {
            audioSourcePlayer = gameObject.AddComponent<AudioSourcePlayer>();
        }

        if (HoverInteractInfo.Count == 0)
        {
            HoverInteractInfo.Add(new InteractInfo(InteractType.Primary, "Pick up"));
        }

        if (HeldInfos.Count == 0)
        {
            HeldInfos.Add(new InteractInfo(InteractType.Primary, "Place"));
        }

    }
    void OnValidate()
    {
        InitRefs();

    }
    void Awake()
    {
        InitRefs();
    }

    // public virtual void OnStartHolding(PlayerItemHolder holder) { }

    // public virtual void OnStopHolding(PlayerItemHolder holder) { }

    // public virtual void OnHeldUpdate(PlayerItemHolder holder, PlacementInfo info) { }

    /// <summary>
    /// Called when the player interacts whilst holding this item. <br/>
    /// Skips default functionality if returning true.<br/>
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public virtual bool OnPressedInteract(PlayerItemHolder holder, InteractionContext context)
    {
        return false;
    }

    /// <summary>
    /// Get the placement preview of this holdable.
    /// </summary>
    /// <param name="holder"></param>
    /// <param name="info"></param>
    /// <param name="pos"></param>
    /// <param name="rot"></param>
    /// <returns></returns>
    public virtual bool TryGetPlacementPreview(PlayerItemHolder holder, PlacementInfo info, out Vector3 pos, out Quaternion rot, out bool show)
    {

        pos = default;
        rot = default;
        show = false;
        return false;
    }

    public virtual bool TryPlace(PlayerItemHolder holder, PlacementInfo info)
    {
        if (TryGetComponent<Snapper>(out var snapper))
        {
            return snapper.TryPlaceFromPlacementInfo(info);
        }

        return false;
    }



    /// <summary>
    /// Returns the root of holdables.
    /// 
    /// Stops at the player hand if the player is holding.
    /// </summary>
    /// <returns></returns>
    public Holdable GetHoldableRoot()
    {

        Transform t = transform;

        while (t.parent != null)
        {
            if (t.parent.GetComponent<PlayerHand>() != null)
            {
                break;
            }

            t = t.parent;
        }
        return t.GetComponentInChildren<Holdable>();
    }

    public bool IsBeingHeld()
    {
        return beingHeld;
    }
    public override void Interact(InteractionContext context)
    {
        OnHeld?.Invoke(context);
        // context.Player.ItemHolder.TryHold(GetHoldableRoot());
        if (context.Type == InteractType.Primary)
        {
            context.Player.ItemHolder.TryHold(this);
        }

        beingHeld = true;
    }


    public virtual List<InteractInfo> GetHeldInteractInfos(PlayerItemHolder holder)
    {
        return HeldInfos;
    }
    /// <summary>
    /// Called by the item holder after this is held
    /// </summary>
    /// <param name="holder"></param>
    public void OnAfterHeld(PlayerItemHolder holder)
    {
        if (ItemData == null) return;

        audioSourcePlayer.SetAudioDef(ItemData.PickUpSound);
        audioSourcePlayer.Play();

        OnAfterHeldInternal(holder);

    }
    /// <summary>
    /// Called by the item holder after this is placed
    /// </summary>
    /// <param name="holder"></param>
    public void OnAfterPlace(PlayerItemHolder holder)
    {
        if (ItemData == null) return;
        audioSourcePlayer.SetAudioDef(ItemData.PlaceSound);
        audioSourcePlayer.Play();

        OnAfterPlaceInternal(holder);
    }

    protected virtual void OnAfterHeldInternal(PlayerItemHolder holder)
    {
    }
    protected virtual void OnAfterPlaceInternal(PlayerItemHolder holder)
    {
    }



    /// <summary>
    /// Set state in this holdable indicating that it is no longer being held
    /// </summary>
    public void SetNotHolding()
    {
        beingHeld = false;
    }

    /// <summary>
    /// Combines linked visuals into one gameobject
    /// </summary>
    /// <param name="visualRoot"></param>
    public GameObject GetParentChildLinkedVisualRootClone()
    {
        var holdables = GetHoldableRoot().GetComponentsInChildren<Holdable>();
        // GameObject visualRoot = Instantiate(GetHoldableRoot().VisualRoot);

        int i = 0;
        GameObject parent = null;

        // uniform for cook material
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        int cookAmountId = Shader.PropertyToID("_CookAmount");
        foreach (Holdable holdable in holdables)
        {
            GameObject holdableVisualRoot = Instantiate(holdable.VisualRoot, holdable.VisualRoot.transform.position, holdable.VisualRoot.transform.rotation);


            // apply cookable material to the clones- with the cook uniform
            if (holdable.TryGetComponent<Ingredient>(out var ingredient) &&
                ingredient.TryGetCookable(out var cookable) != null)
            {
                float cook = cookable.CookNormalized;

                var renderers = holdableVisualRoot.GetComponentsInChildren<Renderer>(true);

                foreach (var r in renderers)
                {
                    mpb.SetFloat(cookAmountId, cook);
                    r.SetPropertyBlock(mpb);
                }
            }



            if (holdableVisualRoot.TryGetComponent<Outline>(out var outline))
            {
                Destroy(outline);
            }
            holdableVisualRoot.layer = LayerMask.NameToLayer("Overlay");

            // set the same material 
            // holdableVisualRoot.GetComponentInChildren<Renderer>().material = holdable.VisualRoot.GetComponentInChildren<Renderer>().material;

            foreach (var obj in holdableVisualRoot.GetComponentsInChildren<Transform>())
            {
                obj.gameObject.layer = LayerMask.NameToLayer("Overlay");
            }
            if (i == 0)
            {
                // parent
                parent = holdableVisualRoot;

            }
            holdableVisualRoot.transform.SetParent(parent.transform, true);
            i++;

        }
        return parent;
    }



#if UNITY_EDITOR
    public float GizmoSize = .3f;
    void OnDrawGizmos()
    {
        if (beingHeld)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        if (VisualRoot == null)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            Handles.Label(transform.position, "VisualRoot not set!", style);
        }

        Gizmos.DrawWireCube(transform.position, Vector3.one * GizmoSize);

    }

#endif
}
