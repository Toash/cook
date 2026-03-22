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
    public NPCScheduleController Schedule;

    public NPCMovement Movement;
    public NPCHand Hand;
    public NPCWorldDialogue Dialogue;
    public CharacterVisual Visuals;

    void Awake()
    {
        if (Brain == null)
        {
            Debug.LogError("[NPC]: Brain is null!");
        }
        if (Schedule == null)
        {
            Debug.LogError("[NPC]: Schedule is null!");
        }
        if (Movement == null)
        {
            Debug.LogError("[NPC]: Movement is null!");
        }
        if (Hand == null)
        {
            Debug.LogError("[NPC]: Hand is null!");
        }
        if (Visuals == null)
        {
            Debug.LogError("[NPC]: Visuals is null!");
        }
    }






}

