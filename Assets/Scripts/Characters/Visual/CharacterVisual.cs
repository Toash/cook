using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;
using RootMotion.FinalIK;
using EasyRoads3Dv3;

/// <summary>
/// Handles the visual representation of a character.
/// </summary>
public class CharacterVisual : MonoBehaviour
{
    public CharacterVisualContextProvider ContextProvider;
    [Header("Properties")]
    [Tooltip("The max walkspeed to base walking animations off of.")]
    public float TargetMaxWalkSpeed = 1.8f;

    // [Header("Eyes")]
    // public Transform LeftEyeBone;
    // public Transform RightEyeBone;
    // public Vector3 EyeForwardLocal = Vector3.up;

    [Header("Eyelids")]

    // Left Eye
    public Transform LeftTopEyelidBone;
    public Transform LeftBottomEyelidBone;

    // Right Eye
    public Transform RightTopEyelidBone;
    public Transform RightBottomEyelidBone;

    [Header("Eyelid Rotations")]

    // Top lids
    public Vector3 TopEyelidOpenLocalRotation = new Vector3(-180, 0, 0);
    public Vector3 TopEyelidClosedLocalRotation = new Vector3(-150, 0, 0);

    // Bottom lids
    public Vector3 BottomEyelidOpenLocalRotation = new Vector3(-180, 0, 0);
    public Vector3 BottomEyelidClosedLocalRotation = new Vector3(150, 0, 0);


    [Header("Blink")]
    public float BlinkSpeed = 0.06f;
    public float BlinkClosedHoldTime = 0.02f;

    [Tooltip("Time between automatic blinks.")]
    public float BlinkRate = 3f;
    [Header("Look At")]
    public float LookAtWeightLerpSpeed = 5f;

    private float currentLookWeight;

    [Header("References")]
    // public Transform VisualRoot;
    public Moveable Moveable;
    public LookAtIK LookAtIK;
    public Animator Animator;
    public string WalkBlendTreeParameter = "Vert";

    private float blinkTimer;
    private Sequence blinkSequence;


    void Start()
    {
        SetEyelidsToOpenImmediate();
        ResetBlinkTimer();
    }

    void Update()
    {
        if (Moveable != null && Animator != null)
        {
            float normalizedSpeed = (float)Moveable.GetMoveSpeed() / TargetMaxWalkSpeed;
            Animator.SetFloat(WalkBlendTreeParameter, normalizedSpeed);
        }

        blinkTimer -= Time.deltaTime;
        if (blinkTimer <= 0f)
        {
            Blink();
            ResetBlinkTimer();
        }


        LookAtIK.solver.target = ResolveLookTarget(ContextProvider.Context);
        // LookAtIK.solver.SetLookAtWeight(ResolveLookTarget(ContextProvider.Context) != null ? 1f : 0f);

        bool hasTarget = ResolveLookTarget(ContextProvider.Context) != null;
        float targetWeight = hasTarget ? 1f : 0f;

        // currentLookWeight = Mathf.Lerp(currentLookWeight, targetWeight, Time.deltaTime * LookAtWeightLerpSpeed);
        currentLookWeight = Mathf.MoveTowards(currentLookWeight, targetWeight, Time.deltaTime * LookAtWeightLerpSpeed);
        LookAtIK.solver.SetLookAtWeight(currentLookWeight);
    }
    private Transform ResolveLookTarget(CharacterVisualContext context)
    {
        if (context == null)
            return null;

        if (context.ForceLookAtTarget != null)
            return context.ForceLookAtTarget;

        if (context.IsEating && context.FoodTarget != null)
            return context.FoodTarget;

        if (context.NearbyPlayer != null)
            return context.NearbyPlayer.Controller.CamRoot;

        return null; // idle fallback
    }

    [Button]
    public void Blink()
    {
        if (!HasAllEyelids())
            return;

        blinkSequence?.Kill();

        blinkSequence = DOTween.Sequence();

        // CLOSE
        blinkSequence.Join(LeftTopEyelidBone.DOLocalRotate(TopEyelidClosedLocalRotation, BlinkSpeed));
        blinkSequence.Join(RightTopEyelidBone.DOLocalRotate(TopEyelidClosedLocalRotation, BlinkSpeed));

        blinkSequence.Join(LeftBottomEyelidBone.DOLocalRotate(BottomEyelidClosedLocalRotation, BlinkSpeed));
        blinkSequence.Join(RightBottomEyelidBone.DOLocalRotate(BottomEyelidClosedLocalRotation, BlinkSpeed));

        blinkSequence.AppendInterval(BlinkClosedHoldTime);

        // OPEN
        blinkSequence.Append(LeftTopEyelidBone.DOLocalRotate(TopEyelidOpenLocalRotation, BlinkSpeed));
        blinkSequence.Join(RightTopEyelidBone.DOLocalRotate(TopEyelidOpenLocalRotation, BlinkSpeed));

        blinkSequence.Join(LeftBottomEyelidBone.DOLocalRotate(BottomEyelidOpenLocalRotation, BlinkSpeed));
        blinkSequence.Join(RightBottomEyelidBone.DOLocalRotate(BottomEyelidOpenLocalRotation, BlinkSpeed));

        blinkSequence.OnKill(() => blinkSequence = null);
    }
    public void SetEyelidsToOpenImmediate()
    {
        LeftTopEyelidBone.localRotation = Quaternion.Euler(TopEyelidOpenLocalRotation);
        RightTopEyelidBone.localRotation = Quaternion.Euler(TopEyelidOpenLocalRotation);

        LeftBottomEyelidBone.localRotation = Quaternion.Euler(BottomEyelidOpenLocalRotation);
        RightBottomEyelidBone.localRotation = Quaternion.Euler(BottomEyelidOpenLocalRotation);
    }

    public void SetEyelidsToClosedImmediate()
    {
        LeftTopEyelidBone.localRotation = Quaternion.Euler(TopEyelidClosedLocalRotation);
        RightTopEyelidBone.localRotation = Quaternion.Euler(TopEyelidClosedLocalRotation);

        LeftBottomEyelidBone.localRotation = Quaternion.Euler(BottomEyelidClosedLocalRotation);
        RightBottomEyelidBone.localRotation = Quaternion.Euler(BottomEyelidClosedLocalRotation);
    }

    private bool HasAllEyelids()
    {
        return LeftTopEyelidBone != null &&
               LeftBottomEyelidBone != null &&
               RightTopEyelidBone != null &&
               RightBottomEyelidBone != null;
    }

    private void ResetBlinkTimer()
    {
        blinkTimer = BlinkRate;
    }

    void OnDisable()
    {
        blinkSequence?.Kill();
        blinkSequence = null;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Transform target = ResolveLookTarget(ContextProvider.Context);
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(target.position, .1f);

        }
    }
#endif

}