using Assets.Scripts.Vehicle;
using UnityEngine;

//lol
public class PlayerVelocityProviderFoodTruckSet : MonoBehaviour
{
    public PlayerTrigger Trigger;
    public FoodTruck Provider;

    void Start()
    {
        Trigger.PlayerEntered.AddListener(PlayerEntered);

    }
    void OnDestroy()
    {
        Trigger.PlayerEntered.RemoveListener(PlayerEntered);

    }

    void PlayerEntered(Player player)
    {
        player.Controller.SetVelocityProvider(Provider);

    }





}