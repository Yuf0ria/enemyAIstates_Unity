using System.Collections;
using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject npcPrefab;
    private const int spawnCount = 15;
    private Vector2 areaSize = new Vector2(5f, 5f);
    
    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 0.5f;
    
    void Start()
    {
        StartCoroutine(SpawnNPCWithDelay());
    }
    
    private IEnumerator SpawnNPCWithDelay()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 spawnPosition = RandomSpawnPos();
            GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
            
            NpcPointsTp_noTraverse npcPath = npc.GetComponent<NpcPointsTp_noTraverse>();
            if (npcPath != null && patrolPoints != null)
            {
                npcPath.points = patrolPoints;
            }
            
            yield return new WaitForSeconds(spawnDelay);
        }
        
        // Tell GameManager that all NPCs are spawned
        if (CountManagerScript.Instance != null)
        {
            CountManagerScript.Instance.NPCSpawningComplete();
        }
        
        Debug.Log("All NPCs spawned!");
    }
    
    private Vector3 RandomSpawnPos()
    {
        var randomX = Random.Range(-areaSize.x / 2, areaSize.x / 2);
        var randomZ = Random.Range(-areaSize.y / 2, areaSize.y / 2);
        return transform.position + new Vector3(randomX, 0, randomZ);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(areaSize.x, 1, areaSize.y));
    }
}