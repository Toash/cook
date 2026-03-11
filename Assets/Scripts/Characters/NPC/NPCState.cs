using Sirenix.OdinInspector;
using UnityEngine;

public abstract class NPCState : MonoBehaviour
{

    [ShowInInspector]
    public abstract string Name { get; }

    [HideInInspector]
    public NPC NPC;
    [HideInInspector]
    public NPCBrain Brain;


    public abstract void OnEnter(NPCBrain brain);
    public abstract void OnUpdate(NPCBrain brain);
    public abstract void OnFixedUpdate(NPCBrain brain);
    public abstract void OnExit(NPCBrain brain);

}