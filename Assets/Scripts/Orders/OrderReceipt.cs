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

    // TODO reference to text in world space ui

    public Snapper Snapper { get; private set; }
    public PhysicsGrabbable Grabbable { get; private set; }

    void OnValidate()
    {
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.isTrigger = true;
        }
    }
    void Awake()
    {
        Grabbable = GetComponent<PhysicsGrabbable>();
        Snapper = GetComponent<Snapper>();
        Snapper.SetSnapType(SnapType.Receipt);
    }
    public void Init(int ID)
    {
        this.OrderID = ID;
        IDText.text = ID.ToString();
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Handles.Label(transform.position, "OrderReceipt with ID: " + OrderID);
    }
#endif
}