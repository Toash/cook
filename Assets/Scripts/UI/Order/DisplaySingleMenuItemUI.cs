using TMPro;
using UnityEngine;

public class DisplaySingleMenuItemUI : MonoBehaviour
{

    public TMP_Text ItemName;

    public Transform RequirementsFlexContainer;
    public DisplaySingleIngredientRequirement DisplaySingleIngredientRequirement;
    [Tooltip("The UI element used to display added condiments for this menu item.")]
    public DisplayAddedCondiments DisplayAddedCondiments;

    private OrderedMenuItem displayedItem;

    public void Populate(OrderedMenuItem item)
    {
        this.displayedItem = item;
        ItemName.text = item.BaseItem.Name;

        // instantiate requirements
        foreach (IngredientRequirement requirement in item.BaseItem.Requirements)
        {
            var reqUI = Instantiate(DisplaySingleIngredientRequirement, RequirementsFlexContainer);
            reqUI.Populate(requirement);
        }


        if (item.AddedCondiments.Count > 0)
        {
            DisplayAddedCondiments.gameObject.SetActive(true);
        }
        else
        {
            DisplayAddedCondiments.gameObject.SetActive(false);
        }
        // display condiments
        DisplayAddedCondiments.Populate(item);


    }


}