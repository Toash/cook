using System.Collections.Generic;
using UnityEngine;

public class NPCSpawnerManager : MonoBehaviour
{
    public static NPCSpawnerManager I { get; private set; }

    [SerializeField] private NPCBrain npcPrefab;
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private int maxNPCCount = 6;
    [SerializeField] private float spawnInterval = 4f;

    private readonly List<NPCBrain> activeNPCs = new();
    private float nextSpawnTime;

    private void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
    }

    private void Update()
    {
        CleanupNullNPCs();

        if (Time.time < nextSpawnTime)
            return;

        nextSpawnTime = Time.time + spawnInterval;

        if (activeNPCs.Count >= maxNPCCount)
            return;

        SpawnNPC();
    }

    private void SpawnNPC()
    {
        if (npcPrefab == null)
        {
            Debug.LogError("[NPCSpawnerManager]: NPC prefab is null.", this);
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogError("[NPCSpawnerManager]: No spawn points assigned.", this);
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        NPCBrain npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        activeNPCs.Add(npc);
    }

    public void UnregisterNPC(NPCBrain npc)
    {
        if (npc == null)
            return;

        activeNPCs.Remove(npc);
    }

    private void CleanupNullNPCs()
    {
        activeNPCs.RemoveAll(npc => npc == null);
    }
}