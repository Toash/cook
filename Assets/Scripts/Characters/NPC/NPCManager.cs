using Assets.Scripts.Vehicle;
using IngameDebugConsole;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager I;
    void Awake()
    {
        if (I != null && I != this)
        {
            DontDestroyOnLoad(gameObject);
            return;
        }
        else if (I == null)
        {
            DontDestroyOnLoad(gameObject);
            I = this;
        }
    }

    public NPC NPC;
    public FoodTruck Truck;


    [ConsoleMethod("SpawnOrderNPC", "Spawns an npc that goes to the food truck to make an order")]
    public static void SpawnNPCGoToTruck()
    {
        if (I == null) return;
        var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        var spawnPoint = player.transform.position + player.Controller.CamRoot.forward * 10;
        spawnPoint.y = 0;

        var npc = GameObject.Instantiate(I.NPC, spawnPoint, Quaternion.identity);
        npc.Brain.CurrentFoodTruck = I.Truck;
        npc.Brain.InitialStateString = "NPCOrderLine";



    }

}