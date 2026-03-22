using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public enum CookState
{
    Raw,
    Cooked,
    Burnt
}

/// <summary>
/// Has state for cook level, and cook states.
/// </summary>
[RequireComponent(typeof(CookableVisual))]
public class Cookable : MonoBehaviour
{
    [Header("Stats")]
    public CookType CookType;
    public float CookedThreshold = 50;
    public float BurntThreshold = 80;
    public float CookRate = 8;



    [ShowInInspector, ReadOnly]
    float cookLevel = 0;
    const float MAX_COOK_LEVEL = 100;
    CookState cookstate = CookState.Raw;
    bool enteredCookedState = false;
    bool enteredBurntState = false;

    public float CookNormalized => Mathf.Clamp01(cookLevel / MAX_COOK_LEVEL);
    public CookState CookState => cookstate;

    void EnterCookedState()
    {
        cookstate = CookState.Cooked;
    }
    void EnterBurntState()
    {
        cookstate = CookState.Burnt;
    }
    /// <summary>
    /// Increase cook state
    /// </summary>
    /// <param name="mult"></param>
    public void Cook(float mult)
    {
        cookLevel = Mathf.Min(cookLevel + CookRate * mult, MAX_COOK_LEVEL);

        if ((cookLevel >= CookedThreshold) && !enteredCookedState)
        {
            EnterCookedState();
            enteredCookedState = true;
        }
        if ((cookLevel >= BurntThreshold) && !enteredBurntState)
        {
            EnterBurntState();
            enteredBurntState = true;
        }
    }


    public void ForceCooked()
    {
        cookLevel = CookedThreshold;

        cookstate = CookState.Cooked;

        enteredCookedState = true;
        enteredBurntState = false;
    }


    public void ForceBurnt()
    {
        cookLevel = BurntThreshold;

        cookstate = CookState.Burnt;

        enteredCookedState = true;
        enteredBurntState = true;
    }

    public void ResetCook()
    {
        cookLevel = 0f;

        cookstate = CookState.Raw;

        enteredCookedState = false;
        enteredBurntState = false;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        string message = "Cook level: " + cookLevel + " / " + MAX_COOK_LEVEL;
        message += "\nCook state: " + cookstate.ToString();
        Handles.Label(transform.position, message);

    }

#endif

}