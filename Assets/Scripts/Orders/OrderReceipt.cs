using TMPro;
using UnityEditor;
using UnityEngine;
/// <summary>
/// World object that the player attaches to a container- to associate it with an active order.
/// 
/// This is required in order to identity an OrderContainer
/// </summary>
[RequireComponent(typeof(Snapper))]
[RequireComponent(typeof(Holdable))]
public class OrderReceipt : MonoBehaviour
{
    public TMP_Text IDText;
    public int OrderID { get; private set; }


    public Snapper Snapper { get; private set; }
    public Holdable Holdable { get; private set; }

    void OnValidate()
    {
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }
    }
    void Awake()
    {
        Snapper = GetComponent<Snapper>();
        Snapper.SetSnapType(SnapType.Receipt);

        Holdable = GetComponent<Holdable>();

        Holdable.HoverTooltipData = new HoverTooltipData(
            transform,
            GetHoverTooltipText
        );
    }
    public void Init(int ID)
    {
        this.OrderID = ID;
        IDText.text = ID.ToString();
    }
    string GetHoverTooltipText()
    {
        if (OrderManager.I == null)
        {
            return $"Order manager null";
        }

        Order order = OrderManager.I.GetActiveOrderFromID(OrderID);
        if (order == null)
        {
            return $"Receipt #{OrderID}\nOrder not found";
        }

        // Replace these fields with whatever your Order actually has.
        // This is just an example shape.
        string text = $"Receipt #{OrderID}";
        foreach (var item in order.MenuItems)
        {
            text += $"\n- {item.Name}";
        }


        return text;
    }


#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, "OrderReceipt with ID: " + OrderID);
    }
#endif
}