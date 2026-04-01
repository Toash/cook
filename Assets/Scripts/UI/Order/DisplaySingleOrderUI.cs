using TMPro;
using UnityEngine;

public class DisplaySingleOrderUI : MonoBehaviour
{

    public TMP_Text OrderNumber;

    public Transform MenuItemsFlexContainer;
    public DisplaySingleMenuItemUI DisplaySingleMenuItemUI;

    public TMP_Text Payout;
    public TMP_Text TimeSinceOrdered;



    private Order displayedOrder;

    void Update()
    {
        if (displayedOrder != null)
        {
            TimeSinceOrdered.text = "Time: " + displayedOrder.TimeSinceOrdered.ToString("F1");
        }
        else
        {
            TimeSinceOrdered.text = "";
        }


    }


    public void Populate(Order order)
    {
        this.displayedOrder = order;

        OrderNumber.text = "Order #: " + order.ID.ToString();
        Payout.text = "$" + order.Payout.ToString();


        foreach (OrderedMenuItem item in order.Items)
        {
            var itemUI = Instantiate(DisplaySingleMenuItemUI, MenuItemsFlexContainer);
            itemUI.Populate(item);

        }

    }


}