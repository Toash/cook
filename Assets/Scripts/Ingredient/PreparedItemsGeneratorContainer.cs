using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Holds all attached (snapped) Prepared Items<br/>
/// <br/>
/// When a ingredient gets snapped, gets all connected ingredients to that, and creates a prepared item for it. <br/>
/// add this prepared item to this containers list.
/// </summary>
[RequireComponent(typeof(Snapper))]
public class PreparedItemsGeneratorContainer : MonoBehaviour
{

    // prepared items that are connected with this container
    public List<PreparedItem> PreparedItems = new();
    private Snapper snapper;

    void Awake()
    {
        snapper = GetComponent<Snapper>();
    }
    void OnEnable()
    {
        snapper.OnSnapEvent += CreatePreparedItem;
        snapper.OnDetachedEvent += DisbandPreparedItem;
    }
    void OnDisable()
    {
        snapper.OnSnapEvent -= CreatePreparedItem;
        snapper.OnDetachedEvent -= DisbandPreparedItem;
    }


    void CreatePreparedItem(Snapper other)
    {
        List<Ingredient> ingredients = new();

        List<Snapper> connectedSnappers = other.ConnectedSnappersDeep();
        foreach (var snapper in connectedSnappers)
        {
            if (snapper.TryGetComponent<Ingredient>(out var ingredient))
            {
                ingredients.Add(ingredient);
            }

        }

        PreparedItem item = PreparedItem.Create(ingredients);

        Debug.Log("[PreparedItemsContainer]: Created prepared item on container");
        PreparedItems.Add(item);
    }

    void DisbandPreparedItem(Snapper other)
    {
        // get prepared item from the other and remove that
        if (other.TryGetComponent<Ingredient>(out var ingredient))
        {
            PreparedItem item = ingredient.PreparedItem;
            PreparedItems.Remove(item);
            item.Disband();

            Debug.Log("[PreparedItemsContainer]: Disbanded prepared item on container");

        }

    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Prepared Item Count: " + PreparedItems.Count.ToString());
    }
#endif
}