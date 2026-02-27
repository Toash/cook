using UnityEditor;
using UnityEngine;


public enum CookState
{
    Raw,
    Cooked,
    Burnt
}

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
public class Cookable : MonoBehaviour
{
    [Header("Stats")]
    public float CookedThreshold = 50;
    public float BurntThreshold = 80;
    public float CookRate = 8;
    [Header("Visuals")]
    public Material CookedMaterial;
    public Material BurntMaterial;


    float cookLevel = 0;
    const float MAX_COOK_LEVEL = 100;
    MeshRenderer renderer;
    CookState cookstate = CookState.Raw;
    bool enteredCookedState = false;
    bool enteredBurntState = false;

    void Awake()
    {
        renderer = GetComponent<MeshRenderer>();
        if (CookedMaterial == null)
        {
            CookedMaterial = Resources.Load<Material>("Materials/DefaultCooked");
        }
        if (BurntMaterial == null)
        {
            BurntMaterial = Resources.Load<Material>("Materials/DefaultBurnt");
        }
    }

    void EnterCookedState()
    {
        cookstate = CookState.Cooked;
        renderer.material = CookedMaterial;
    }
    void EnterBurntState()
    {
        cookstate = CookState.Burnt;
        renderer.material = BurntMaterial;
    }
    public void Cook(float mult)
    {
        cookLevel += CookRate * mult;

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



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        string message = "Cook level: " + cookLevel + " / " + MAX_COOK_LEVEL;
        message += "\nCook state: " + cookstate.ToString();
        Handles.Label(transform.position, message);

    }

#endif

}