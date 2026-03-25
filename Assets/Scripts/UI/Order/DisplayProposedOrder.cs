
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// Displays all the orders from the order manager, based on its events
/// </summary>
public class DisplayProposedOrder : MonoBehaviour
{

    public Transform MenuItemsFlexContainer;
    public DisplaySingleMenuItemUI DisplaySingleMenuItemUI;

    public TMP_Text Payout;
    void Start()
    {
        OrderManager.I.ProposedOrderAdded += OnAddProposedOrder;
        OrderManager.I.ProposedOrderRemoved += OnRemoveProposedOrder;
        Clear();
    }
    void OnDestroy()
    {
        OrderManager.I.ProposedOrderAdded -= OnAddProposedOrder;
        OrderManager.I.ProposedOrderRemoved -= OnRemoveProposedOrder;
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
        foreach (OrderedMenuItem item in proposition.Items)
        {

            DisplaySingleMenuItemUI itemUI = Instantiate(DisplaySingleMenuItemUI, MenuItemsFlexContainer);
            itemUI.Populate(item);

        }

    }


}