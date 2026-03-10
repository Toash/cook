using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
/// <summary>
/// Spawns ingredients based off of their Scriptable Objects.
/// </summary>
public class IngredientSpawner : MonoBehaviour
{
    public static IngredientSpawner I;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
    }



    public void SpawnIngredient(IngredientData data, Vector3 pos)
    {
        GameObject.Instantiate(data.HoldablePrefab, pos, Quaternion.identity);

    }


    [Header("Debug")]
    public IngredientData debugIngredient;
    public Vector3 debugSpawnPos;
    [Button]
    void DebugSpawn()
    {
        SpawnIngredient(debugIngredient, debugSpawnPos);

    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {

        Gizmos.DrawSphere(debugSpawnPos, .2f);
        if (debugIngredient != null)
        {
            Handles.Label(debugSpawnPos, debugIngredient.Name);
        }
    }
#endif



}