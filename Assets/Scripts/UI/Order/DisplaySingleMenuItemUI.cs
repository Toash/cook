using TMPro;
using UnityEngine;

public class DisplaySingleMenuItemUI : MonoBehaviour
{

    public TMP_Text ItemName;

    public Transform RequirementsFlexContainer;
    public DisplaySingleIngredientRequirement DisplaySingleIngredientRequirement;

    private MenuItem displayedItem;

    public void Populate(MenuItem item)
    {
        this.displayedItem = item;
        ItemName.text = item.Name;

        // instantiate requirements
        foreach (IngredientRequirement requirement in item.Requirements)
        {
            var reqUI = Instantiate(DisplaySingleIngredientRequirement, RequirementsFlexContainer);
            reqUI.Populate(requirement);
        }


    }


}