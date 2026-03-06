
using UnityEngine;
/// <summary>
/// Represents a connection between two snappers.
/// </summary>
[System.Serializable]
public class SnapConnection
{
    // public Snapper This;
    // public Snapper Other;
    // public bool IsOwner;

    public Snapper Parent { get; private set; }

    public SnapConnection(Snapper parentSnapper)
    {
        Parent = parentSnapper;
    }
    // public SnapConnection(Snapper thisSnapper, Snapper otherSnapper, bool isOwner)
    // {
    //     This = thisSnapper;
    //     Other = otherSnapper;
    //     IsOwner = isOwner;
    // }

    // public override string ToString()
    // {
    //     return "SnapConnection: \n" +
    //     "This: " + This.gameObject.name + "\n" +
    //     "Other: " + Other.gameObject.name + "\n" +
    //     "IsOwner: " + IsOwner;
    // }
    // public static Snapper GetThis(SnapConnection connection)
    // {
    //     Snapper thisSnapper;
    //     if (connection.IsOwner == true)
    //     {
    //         thisSnapper = connection.This;
    //     }
    //     else
    //     {
    //         thisSnapper = connection.Other;
    //     }
    //     return thisSnapper;
    // }
    // public static Snapper GetOther(SnapConnection connection)
    // {
    //     Snapper otherSnapper;
    //     if (connection.IsOwner == true)
    //     {
    //         otherSnapper = connection.Other;
    //     }
    //     else
    //     {
    //         otherSnapper = connection.This;
    //     }
    //     return otherSnapper;
    // }

}