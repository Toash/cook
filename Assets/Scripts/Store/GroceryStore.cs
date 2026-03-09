using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;



/// <summary>
/// Contains method to buy ingredients, and instantiate those in the world.
/// </summary>
public class GroceryStore : MonoBehaviour
{
    // prefab for spawning in containers
    public HoldableContainer Container;
    public List<GroceryStoreItem> Inventory;


    public LoadingArea LoadingArea;

    public event Action OnBuy;

    void OnValidate()
    {
        HashSet<GroceryStoreItem> visited = new();
        foreach (var item in Inventory)
        {
            if (visited.Contains(item))
            {
                Debug.LogError("Duplicate items");
            }

            visited.Add(item);
        }

    }

    public void BuyItem(GroceryStoreItem item)
    {
        // TODO update stock...


        if (MoneyManager.I.TryTake(item.Price))
        {
            Debug.Log("[Store]: Bought " + item.Ingredient.Name);
            //bought
            // Instantiate(item.Ingredient, LoadingArea.transform.position, Quaternion.identity);



            // create holdable conatiner
            if (item.Ingredient.Prefab.TryGetComponent<Holdable>(out var holdable))
            {
                HoldableContainer container = Instantiate(Container, LoadingArea.transform.position, Quaternion.identity);
                container.Init(holdable, item.Amount);
                OnBuy?.Invoke();
            }
            else
            {
                Debug.LogError("[GroceryStore]: holdable on ingredient is null.");
            }

        }
        else
        {
            Debug.Log("[Store]: Not enough money");
            // not enough money. 
        }

    }

    [Header("Debug")]
    public GroceryStoreItem DebugStoreItem;
    [Button]
    public void DebugBuyItem()
    {
        BuyItem(DebugStoreItem);
    }

}