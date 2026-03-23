//This Script counts the npc spawned in the field
//once spawned it record how many spawn in total.
//the total count then becomes the score base when having a tug of war with the enemy
//SpawnCounts > Spawn Done Get Total Count > No taken by player or Enemy /Total Count
//UGh brain no work anymore I', sleepy

using System.Collections;
using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject npcPrefab;
    private const int spawnCount = 15; //Total Spawns in eahc gameobject
    private Vector2 areaSize = new Vector2(5f, 5f); //Area of eahc gmae object that spawns
    
    [Header("Patrol Points")]
    [SerializeField] private Transform[] patrolPoints;// what they patrol
    
    [Header("Spawn Settings")]
    [SerializeField] private float spawnDelay = 0.5f;//so that they won't all spawn at the same time; don't judge I like variety
    
    void Start()
    {
        StartCoroutine(SpawnNPC());
    }
    
    private IEnumerator SpawnNPC()
    {
        //For every
        for (int i = 0; i < spawnCount; i++)
        {
            //SpawmHEret
            Vector3 spawnPosition = RandomSpawnPos();
            GameObject npc = Instantiate(npcPrefab, spawnPosition, Quaternion.identity);
            
            //Get Component
            NpcPointsTp_noTraverse npcPath = npc.GetComponent<NpcPointsTp_noTraverse>();
            if (npcPath != null && patrolPoints != null)
            {
                npcPath.points = patrolPoints;
            }
            
            yield return new WaitForSeconds(spawnDelay);
        }
        
        //if npc done, get checked in Count Manager Script
        if (CountManagerScript.Instance != null)
        {
            CountManagerScript.Instance.SpawnComplete();
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