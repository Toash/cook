using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Snapper))]
public class Trashcan : MonoBehaviour
{
    public Snapper Snapper;
    void InitRef()
    {

        if (Snapper == null)
        {
            Snapper = GetComponent<Snapper>();
            Snapper.SetSnapType(SnapType.Container);
        }
    }

    void Awake()
    {
        InitRef();
    }
    void Start()
    {
        InitRef();
    }

    void OnEnable()
    {
        Snapper.OnChildSnapped += OnSnapped;
    }
    void OnDisable()
    {
        Snapper.OnChildSnapped -= OnSnapped;
    }


    void OnSnapped(Snapper child)
    {
        // child.DetachFromParent();

        Destroy(child.gameObject);

    }

}