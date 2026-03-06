using UnityEngine;

/// <summary>
/// an NPC instance in the world.
/// </summary>
public class NPC : MonoBehaviour
{
    public string NPCName = "Timmy";

    public NPCBrain Brain;
    public CharacterVisual Visuals;



    void Awake()
    {
        Brain = GetComponent<NPCBrain>();
    }
}

