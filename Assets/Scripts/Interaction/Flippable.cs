using System.Collections;
using UnityEngine;

public class Flippable : MonoBehaviour
{

    [Header("State")]
    public bool IsFrontSideUp { get; private set; } = true;

    private bool isFlipping;

    public bool CanFlip()
    {
        if (isFlipping) return false;

        return true;
    }

    public bool TryFlip()
    {
        if (!CanFlip()) return false;

        Debug.Log("[Flippable]: flipped");
        IsFrontSideUp = !IsFrontSideUp;


        OnFlipped();

        return true;
    }


    private void OnFlipped()
    {
    }
}