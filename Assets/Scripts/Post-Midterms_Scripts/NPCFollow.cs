using UnityEngine;

public class NPCFollow : MonoBehaviour
{
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private Color playerColor = Color.blue;
    [SerializeField] private Color enemyColor = Color.red;
    
    private Transform player, enemy;
    private Renderer npcRenderer;
    private Color originalColor;
    private NpcPointsTp_noTraverse NPCPoints;
    
    private Transform currentTarget;
    private bool hasBeenConverted = false;
    private string currentOwner = "";

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemy = GameObject.FindGameObjectWithTag("Enemy")?.transform;
        npcRenderer = GetComponent<Renderer>();
        NPCPoints = GetComponent<NpcPointsTp_noTraverse>();
        
        if (npcRenderer != null)
        {
            originalColor = npcRenderer.material.color;
        }
    }

    void Update()
    {
        if (player == null && enemy == null) return;
        
        CheckTargets();
        
        if (currentTarget != null)
        {
            FollowTarget(currentTarget);
        }
    }
    
    private void CheckTargets()
    {
        bool playerInRange = false;
        bool enemyInRange = false;
        
        if (player != null)
        {
            playerInRange = Vector3.Distance(transform.position, player.position) <= detectionRange;
        }
        
        if (enemy != null)
        {
            enemyInRange = Vector3.Distance(transform.position, enemy.position) <= detectionRange;
        }
        
        // Check if someone is trying to steal this NPC
        if (playerInRange && player != null)
        {
            // Player is in range - convert to player
            if (currentOwner != "Player")
            {
                SetTarget(player, "Player");
            }
            else
            {
                // Already player's NPC, just keep following
                currentTarget = player;
            }
        }
        else if (enemyInRange && enemy != null)
        {
            // Enemy is in range - convert to enemy
            if (currentOwner != "Enemy")
            {
                SetTarget(enemy, "Enemy");
            }
            else
            {
                // Already enemy's NPC, just keep following
                currentTarget = enemy;
            }
        }
        else
        {
            // No one in range - follow current owner if converted
            if (hasBeenConverted)
            {
                if (currentOwner == "Player" && player != null)
                {
                    currentTarget = player;
                }
                else if (currentOwner == "Enemy" && enemy != null)
                {
                    currentTarget = enemy;
                }
                
                if (NPCPoints != null)
                {
                    NPCPoints.enabled = false;
                }
            }
            else
            {
                // Not converted yet, go back to patrol
                currentTarget = null;
                if (NPCPoints != null)
                {
                    NPCPoints.enabled = true;
                }
            }
        }
    }
    
    private void SetTarget(Transform newTarget, string newOwner)
    {
        currentTarget = newTarget;
        currentOwner = newOwner;
        hasBeenConverted = true;
        
        if (newOwner == "Player")
        {
            ChangeColor(playerColor);
        }
        else if (newOwner == "Enemy")
        {
            ChangeColor(enemyColor);
        }
        
        if (NPCPoints != null)
        {
            NPCPoints.enabled = false;
        }
    }
    
    private void FollowTarget(Transform target)
    {
        if (target == null) return;
        
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        transform.LookAt(target);
    }

    private void ChangeColor(Color newColor)
    {
        if (npcRenderer != null)
        {
            npcRenderer.material.color = newColor;
        }
    }
    
    public string GetCurrentOwner()
    {
        return currentOwner;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        #if UNITY_EDITOR
        if (hasBeenConverted)
        {
            Vector3 labelPos = transform.position + Vector3.up * 2f;
            UnityEditor.Handles.Label(labelPos, $"Owner: {currentOwner}");
        }
        #endif
    }
}