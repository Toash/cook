using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySingleIngredientRequirement : MonoBehaviour
{

    public TMP_Text Name;

    public RawImage Image;

    public TMP_Text Count;


    private IngredientRequirement requirement = null;

    public void Populate(IngredientRequirement requirement)
    {
        this.requirement = requirement;

        Image.texture = requirement.Data.Image;

        // Name.text = requirement.Data.Name;
        Count.text = requirement.Count.ToString();

    }

}