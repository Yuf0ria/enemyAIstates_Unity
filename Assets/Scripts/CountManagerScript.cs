using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CountManagerScript : MonoBehaviour
{
    public static CountManagerScript Instance;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultText; 
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI enemyCountText;
    
    private int totalNPCs = 0;
    private bool gameEnded = false;
    private bool spawningComplete = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        UpdateUI();
    }

    // Called by NPCSpawn when all NPCs are spawned
    public void NPCSpawningComplete()
    {
        NPCFollow[] allNPCs = FindObjectsByType<NPCFollow>(FindObjectsSortMode.None);
        totalNPCs = allNPCs.Length;
        spawningComplete = true;
        Debug.Log($"Total NPCs found: {totalNPCs}");
    }

    void Update()
    {
        if (!gameEnded && spawningComplete)
        {
            CountActiveFollowers();
            CheckConditions();
        }
    }

    private void CountActiveFollowers()
    {
        int playerCount = 0;
        int enemyCount = 0;
        
        NPCFollow[] allNPCs = FindObjectsByType<NPCFollow>(FindObjectsSortMode.None);
        
        foreach (NPCFollow npc in allNPCs)
        {
            string owner = npc.GetCurrentOwner();
            if (owner == "Player")
            {
                playerCount++;
            }
            else if (owner == "Enemy")
            {
                enemyCount++;
            }
        }
        
        UpdateUI(playerCount, enemyCount);
    }

    private void UpdateUI(int playerFollowers, int enemyFollowers)
    {
        if (playerCountText != null)
            playerCountText.text = $"{playerFollowers}";
        
        if (enemyCountText != null)
            enemyCountText.text = $"{enemyFollowers}";
    }
    
    private void UpdateUI()
    {
        UpdateUI(0, 0);
    }

    private void CheckConditions()
    {
        int playerFollowers = 0;
        int enemyFollowers = 0;
        
        NPCFollow[] allNPCs = FindObjectsByType<NPCFollow>(FindObjectsSortMode.None);
        
        foreach (NPCFollow npc in allNPCs)
        {
            string owner = npc.GetCurrentOwner();
            if (owner == "Player")
            {
                playerFollowers++;
            }
            else if (owner == "Enemy")
            {
                enemyFollowers++;
            }
        }
        
        // Only end game if ALL NPCs have been converted
        if (playerFollowers + enemyFollowers >= totalNPCs)
        {
            gameEnded = true;
            
            if (playerFollowers > enemyFollowers)
            {
                WinCondition();
            }
            else if (enemyFollowers > playerFollowers)
            {
                LoseCondition();
            }
            else
            {
                DrawCondition();
            }
        }
    }

    private void WinCondition()
    {
        if (resultText != null)
            resultText.text = "You've Won!";
        ShowGameOver();
    }

    private void LoseCondition()
    {
        if (resultText != null)
            resultText.text = "You've Lost!";
        ShowGameOver();
    }

    private void DrawCondition()
    {
        if (resultText != null)
            resultText.text = "It's a Draw!";
        ShowGameOver();
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDrawGizmos()
    {
        Vector3 textPosition = transform.position + Vector3.up * 3f;
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(textPosition, 0.5f);
        
        #if UNITY_EDITOR
        if (spawningComplete)
        {
            UnityEditor.Handles.Label(textPosition, $"Total NPCs: {totalNPCs}");
        }
        #endif
    }
}