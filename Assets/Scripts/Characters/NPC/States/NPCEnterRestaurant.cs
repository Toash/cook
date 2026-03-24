using System.Collections;
using UnityEngine;

/// <summary>
/// NPC has decided to enter the restaurant and walk to the order line.
/// </summary>
public class NPCEnterRestaurant : NPCState
{
    public override string Name => "NPCEnterRestaurant";

    private Coroutine enterRoutine;

    private OrderStation Station => Brain.CurrentOrderStation;
    private OrderLine Line => Station != null ? Station.OrderLine : null;

    public override void OnEnter(NPCBrain brain)
    {
        if (Station == null)
        {
            Debug.LogError("[NPCEnterRestaurant]: Brain.CurrentOrderStation is null.");
            Brain.ChangeState("NPCSidewalkWander");
            return;
        }

        if (Line == null)
        {
            Debug.LogError("[NPCEnterRestaurant]: Station.OrderLine is null.");
            Brain.ChangeState("NPCSidewalkWander");
            return;
        }

        if (!Line.CanJoinLine())
        {
            Brain.ClearOrderStation();
            Brain.ChangeState("NPCSidewalkWander");
            return;
        }

        enterRoutine = StartCoroutine(EnterRestaurantCoroutine());
    }

    public override void OnExit(NPCBrain brain)
    {
        if (enterRoutine != null)
        {
            StopCoroutine(enterRoutine);
            enterRoutine = null;
        }
    }

    public override void OnUpdate(NPCBrain brain)
    {
    }

    public override void OnFixedUpdate(NPCBrain brain)
    {
    }

    private IEnumerator EnterRestaurantCoroutine()
    {
        Vector3 targetPos = GetLineEntryPosition();

        yield return NPC.Movement.MoveAndAwait(targetPos);

        enterRoutine = null;
        Brain.ChangeState("NPCOrderLine");
    }

    private Vector3 GetLineEntryPosition()
    {

        return Station.OrderLineEntryPoint.position;

    }

}