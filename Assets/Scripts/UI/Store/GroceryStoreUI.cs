using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GroceryStoreUI : MonoBehaviour
{
    // the store to display items from
    public GroceryStore GroceryStore;

    [Tooltip("The object to spawn the grocery store items on.")]
    public GameObject Container;
    public DisplaySingleGroceryStoreItemUI ItemUI;

    void OnEnable()
    {
        SetUI();
        GroceryStore.OnBuy += SetUI;
    }
    void OnDisable()
    {
        GroceryStore.OnBuy -= SetUI;
    }



    public void SetUI()
    {
        Clear();
        foreach (GroceryStoreItem item in GroceryStore.Inventory)
        {
            DisplaySingleGroceryStoreItemUI ui_inst = Instantiate(ItemUI, Container.transform);
            ui_inst.Populate(GroceryStore, item);
        }
    }


    void Clear()
    {
        for (int i = Container.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(Container.transform.GetChild(i).gameObject);
        }
    }


}