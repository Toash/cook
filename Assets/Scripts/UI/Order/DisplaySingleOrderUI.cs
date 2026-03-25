using TMPro;
using UnityEngine;

public class DisplaySingleOrderUI : MonoBehaviour
{

    public TMP_Text OrderNumber;

    public Transform MenuItemsFlexContainer;
    public DisplaySingleMenuItemUI DisplaySingleMenuItemUI;

    public TMP_Text Payout;


    private Order displayedOrder;



    public void Populate(Order order)
    {
        this.displayedOrder = order;

        OrderNumber.text = "Order #: " + order.ID.ToString();
        Payout.text = "$" + order.Payout.ToString();


        foreach (var item in order.Items)
        {

            var itemUI = Instantiate(DisplaySingleMenuItemUI, MenuItemsFlexContainer);
            itemUI.Populate(item);

        }

    }


}