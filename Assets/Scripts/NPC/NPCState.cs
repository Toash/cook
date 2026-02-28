using UnityEngine;

public abstract class NPCState : MonoBehaviour
{
    public abstract string StateName { get; }
    public abstract void OnEnter(NPCBrain brain);
    public abstract void OnUpdate(NPCBrain brain);
    public abstract void OnFixedUpdate(NPCBrain brain);
    public abstract void OnExit(NPCBrain brain);

}