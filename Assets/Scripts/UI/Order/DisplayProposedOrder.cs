
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Displays all the orders from the order manager, based on its events
/// </summary>
public class DisplayProposedOrder : MonoBehaviour
{
    public OrderManager OrderManager;

    public Transform MenuItemsFlexContainer;
    public DisplaySingleMenuItemUI DisplaySingleMenuItemUI;

    public TMP_Text Payout;
    void OnEnable()
    {
        OrderManager.ProposedOrderAdded += OnAddProposedOrder;
        OrderManager.ProposedOrderRemoved += OnRemoveProposedOrder;
    }
    void OnDisable()
    {
        OrderManager.ProposedOrderAdded -= OnAddProposedOrder;
        OrderManager.ProposedOrderRemoved -= OnRemoveProposedOrder;
    }

    void Start()
    {
        Clear();
    }


    void OnAddProposedOrder(OrderProposition order)
    {
        Clear();
        Populate(OrderManager.I.ProposedOrder);
    }
    void OnRemoveProposedOrder(OrderProposition order)
    {
        Clear();
    }


    void Clear()
    {
        for (int i = MenuItemsFlexContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(MenuItemsFlexContainer.transform.GetChild(i).gameObject);
        }

        Payout.text = "";

    }

    public void Populate(OrderProposition proposition)
    {
        Payout.text = "$" + proposition.Payout.ToString();
        foreach (var item in proposition.MenuItems)
        {

            var itemUI = Instantiate(DisplaySingleMenuItemUI, MenuItemsFlexContainer);
            itemUI.Populate(item);

        }

    }


}