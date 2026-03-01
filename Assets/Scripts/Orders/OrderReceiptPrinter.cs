using UnityEditor;
using UnityEngine;

public class OrderReceiptPrinter : MonoBehaviour
{
    public OrderReceipt ReceiptPrefab;
    public Transform SpawnLocation;


    void Start()
    {
        OrderManager.I.ProposedOrderAcknowledged += OnProposedOrderAcknowledged;
    }
    void OnDisable()
    {
        OrderManager.I.ProposedOrderAcknowledged -= OnProposedOrderAcknowledged;
    }


    void OnProposedOrderAcknowledged(Order order)
    {
        SpawnReceipt(order.ID);
    }


    void SpawnReceipt(int ID)
    {
        OrderReceipt inst = Instantiate(ReceiptPrefab, SpawnLocation.position, SpawnLocation.rotation);
        inst.Init(ID);
    }



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "Receipt printer");
        if (SpawnLocation != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(SpawnLocation.position, .2f);
        }
    }
#endif
}