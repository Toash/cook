using Assets.Scripts.Characters.NPC;
using UnityEngine;

/// <summary>
/// an NPC instance in the world.
/// 
/// Contains all of the subsystems of an npc.
/// </summary>
public class NPC : MonoBehaviour
{

    public NPCBrain Brain;

    public NPCMovement Movement;
    public NPCHand Hand;
    public NPCWorldDialogue Dialogue;

    void Awake()
    {
        if (Brain == null)
        {
            Debug.LogError("[NPC]: Brain is null!");
        }
        if (Movement == null)
        {
            Debug.LogError("[NPC]: Movement is null!");
        }
        if (Hand == null)
        {
            Debug.LogError("[NPC]: Hand is null!");
        }
    }






}

