using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public static TableManager I;

    [SerializeField] private List<Transform> seats = new();
    private HashSet<Transform> occupied = new();

    private void Awake()
    {
        I = this;
    }

    public Transform GetAvailableSeat()
    {
        foreach (var seat in seats)
        {
            if (!occupied.Contains(seat))
                return seat;
        }

        return null;
    }

    public void ReserveSeat(Transform seat)
    {
        occupied.Add(seat);
    }

    public void ReleaseSeat(Transform seat)
    {
        occupied.Remove(seat);
    }
}