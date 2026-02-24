using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Cookable : MonoBehaviour
{
    public float CookedThreshold = 50;
    public float BurntThreshold = 80;
    public float CookRate = 4;

    float cookLevel = 0;
    const float MAX_COOK_LEVEL = 100;

    public void Cook(float mult)
    {
        cookLevel += CookRate * mult;
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Cook level: " + cookLevel + " / " + MAX_COOK_LEVEL);
    }

#endif

}