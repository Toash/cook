using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Source of truth for an order submission<br/>
/// Holds all attached (snapped) Prepared Items<br/>
/// <br/>
/// When a ingredient gets snapped, gets all connected ingredients to that, and creates a prepared item for it. <br/>
/// add this prepared item to this containers list.
/// </summary>
[RequireComponent(typeof(Snapper))]
public class OrderContainer : MonoBehaviour
{

    // prepared items that are connected with this container
    public List<PreparedItem> PreparedItems = new();
    private Snapper snapper;

    void Awake()
    {
        snapper = GetComponent<Snapper>();
        snapper.JointPriority = JointPriority.CONTAINER;
    }
    void OnEnable()
    {
        snapper.OnSnapEvent += CreatePreparedItem;
        snapper.OnDetachedEvent += DetachPreparedItem;
    }
    void OnDisable()
    {
        snapper.OnSnapEvent -= CreatePreparedItem;
        snapper.OnDetachedEvent -= DetachPreparedItem;
    }

    void CreatePreparedItem(SnapConnection connection)
    {
        List<Ingredient> ingredients = new();

        List<Snapper> connectedSnappers = connection.Other.GetSnapperGroup();
        foreach (var snapper in connectedSnappers)
        {
            if (snapper.TryGetComponent<Ingredient>(out var ingredient))
            {
                if (ingredient.PreparedItem != null) continue;
                Debug.Log("[OrderContainer]: Creating prepared item with ingredient: " + ingredient);
                ingredients.Add(ingredient);
            }

        }

        PreparedItem item = PreparedItem.Create(ingredients);
        // item.transform.SetParent(gameObject.transform);

        Debug.Log("[PreparedItemsContainer]: Created prepared item on container");
        PreparedItems.Add(item);
    }

    void DetachPreparedItem(SnapConnection connection)
    {
        // get prepared item from the other and remove that
        if (connection.Other.TryGetComponent<Ingredient>(out var ingredient))
        {
            PreparedItem preparedItem = ingredient.PreparedItem;
            // preparedItem.transform.SetParent(null);
            PreparedItems.Remove(preparedItem);

            preparedItem.Disband();

            Debug.Log("[PreparedItemsContainer]: Disbanded prepared item on container");

        }

    }
    public static bool ContainsContainerInGroup(List<Snapper> snappers)
    {
        foreach (var snapper in snappers)
        {
            if (snapper.GetComponent<OrderContainer>() != null)
            {
                return true;
            }
        }
        return false;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Prepared Item Count: " + PreparedItems.Count.ToString());
    }
#endif
}