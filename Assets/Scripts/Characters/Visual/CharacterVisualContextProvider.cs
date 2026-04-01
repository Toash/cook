using Sirenix.OdinInspector;
using UnityEngine;
/// <summary>
/// Provides context to character visual 
/// </summary>
public class CharacterVisualContextProvider : MonoBehaviour
{
    public NearbyPlayerTrigger PlayerTrigger;

    public CharacterVisualContext Context = new();

    void Update()
    {
        Context.NearbyPlayer = PlayerTrigger != null
            ? PlayerTrigger.NearbyPlayer
            : null;
    }
}