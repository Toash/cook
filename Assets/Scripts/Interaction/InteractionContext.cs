using System.Numerics;
using UnityEngine;

public class InteractionContext
{
    public InteractType Type;
    public Player Player;


    public InteractionContext(InteractType type, Player player)
    {
        this.Type = type;
        this.Player = player;
    }

    public override string ToString()
    {
        return $"[InteractionContext]: Type: {Type}, Player: {Player.name}";
    }

}