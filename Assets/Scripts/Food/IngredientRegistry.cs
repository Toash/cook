using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IngredientData
{
    public IngredientType Type;
    public Texture2D Image;
}
/// <summary>
/// Contains data for each ingredient type.
/// </summary>
public class IngredientRegistry : MonoBehaviour
{
    public static IngredientRegistry I;

    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }
    public List<IngredientData> IngredientDatas = new List<IngredientData>();

}