using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A building that NPCS can enter in. 
/// </summary>
public class EnterableBuilding : MonoBehaviour
{
    public List<NPC> Occupants = new List<NPC>();



    /// <summary>
    /// Place that NPCS should move to and appear after leaving.
    /// </summary>
    public Transform Entrance;

}