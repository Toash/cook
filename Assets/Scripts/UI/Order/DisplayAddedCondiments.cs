using Unity.Multiplayer.Center.Common;
using UnityEngine;

public class DisplayAddedCondiments : MonoBehaviour
{
    public DisplaySingleCondiment DisplaySingleCondiment;
    public Transform CondimentsFlexContainer;

    public void Populate(OrderedMenuItem orderedMenuItem)
    {
        // Clear previous condiments
        foreach (Transform child in CondimentsFlexContainer)
        {
            Destroy(child.gameObject);
        }

        // Display added condiments
        foreach (CondimentData condiment in orderedMenuItem.AddedCondiments)
        {
            DisplaySingleCondiment displayCondiment = Instantiate(DisplaySingleCondiment, CondimentsFlexContainer);
            displayCondiment.Populate(condiment);
        }


    }
}