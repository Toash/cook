using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySingleIngredientRequirement : MonoBehaviour
{

    public TMP_Text Count;

    public RawImage Image;

    // public TMP_Text Count;



    public void Populate(IngredientRequirement requirement)
    {

        Image.texture = requirement.Data.Image;

        Count.text = requirement.Count.ToString();

    }

}