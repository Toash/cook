using Assets.Scripts.Characters.NPC;
using UnityEngine;

/// <summary>
/// an NPC instance in the world.
/// </summary>
public class NPC : MonoBehaviour
{
    //public string NPCName = "Timmy";

    public NPCBrain Brain;
    public NPCSchedule Schedule;

    public NPCMovement Movement;
    public NPCHand Hand;
    public CharacterVisual Visuals;



    void Awake()
    {
    }
}

