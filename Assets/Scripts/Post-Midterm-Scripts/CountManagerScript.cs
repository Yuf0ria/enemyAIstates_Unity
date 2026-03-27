//More like UI Count Manager Script
//This Checks how many are at the players side vs the enemy
//if player more npc, player win
//if enemy more npc, player lose
//tugg-tuggh tigity tug og awr
//update: Shortened the Methods
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
    public void SpawnComplete()
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
            Counter(); //Counts the no of npc got by player or enemy
            Conditions();
        }
    }

    private void Counter()
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

    private void Conditions()
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
        
        //if player and enemy got them all
        if (playerFollowers + enemyFollowers >= totalNPCs)
        {
            gameEnded = true;
            
            //if player is more than enemy
            if (playerFollowers > enemyFollowers)
            {
                Win();
            }
            else if (enemyFollowers > playerFollowers) //if player less than enemy
            {
                Lose();
            }
            else //yeah it tie
            {
                Draw();
            }
        }
    }

    private void Win()
    {
        if (resultText != null)
            resultText.text = "You've Won!";
        GameOver();
    }

    private void Lose()
    {
        if (resultText != null)
            resultText.text = "You've Lost!";
        GameOver();
    }

    private void Draw()
    {
        if (resultText != null)
            resultText.text = "It's a Draw!";
        GameOver();
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
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