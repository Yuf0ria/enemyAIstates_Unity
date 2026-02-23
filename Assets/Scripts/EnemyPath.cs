#region Assemblies
    using UnityEngine;
    using UnityEngine.AI;
#endregion
//Enemy targets and chases NPCs instead of patrol points
public class EnemyPath : MonoBehaviour
{
    #region Components in Inspector
        [Header("Enemy Agent")]
        [SerializeField] public NavMeshAgent _agent;
        [Header("Movement Speed")]
        [SerializeField] private float enemySpeed = 5f;
        [Header("Detection")]
        [SerializeField] private float detectionRadius = 15f; // How far enemy can see NPCs
    #endregion
    #region Not in Inspector
        private Transform targetNPC;
        // private float _radius = 6f;
    #endregion
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.speed = enemySpeed;
    }
    
    private void Update()
    {
        FindAndChaseNPC();
    }
    
    #region Logic.EXE
        private void FindAndChaseNPC()
        {
            // Find all NPCs in the scene
            NPCFollow[] allNPCs = FindObjectsByType<NPCFollow>(FindObjectsSortMode.None);
            
            Transform closestNPC = null;
            float closestDistance = detectionRadius;
            
            // Find the closest NPC within detection range
            foreach (NPCFollow npc in allNPCs)
            {
                float distance = Vector3.Distance(transform.position, npc.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNPC = npc.transform;
                }
            }
            
            // Chase the closest NPC or wander if none found
            if (closestNPC != null)
            {
                targetNPC = closestNPC;
                _agent.SetDestination(targetNPC.position);
            }
            else
            {
                //Wander randomly if no NPCs
                if (!_agent.hasPath || _agent.remainingDistance < 0.5f)
                {
                    Wander();
                }
            }
        }
        
        private void Wander()
        {
            // Pick a random point to walk to
            Vector3 randomDirection = Random.insideUnitSphere * 10f;
            randomDirection += transform.position;
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
            
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            
            // Draw line to current target
            if (targetNPC != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetNPC.position);
            }
        }   
    #endregion
}