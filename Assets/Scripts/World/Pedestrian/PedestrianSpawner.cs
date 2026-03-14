using IngameDebugConsole;
using UnityEngine;

public class PedestrianSpawner : MonoBehaviour
{


    public static PedestrianSpawner I;
    public PedestrianGraph Graph;
    public GameObject SpawnRoot;
    public int InitialAmount = 30;
    public GameObject NPCPrefab;
    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;


    }
    void Start()
    {
        Spawn(InitialAmount);

    }


    [ConsoleMethod("SpawnPed", "Spawns pedestrians")]
    public static void Spawn(int amount)
    {
        if (I == null) return;
        for (int i = 0; i < amount; i++)
        {
            var npc = Instantiate(I.NPCPrefab, I.Graph.GetRandomNode().transform.position, Quaternion.identity);
            npc.transform.SetParent(I.SpawnRoot.transform);
        }

    }

}