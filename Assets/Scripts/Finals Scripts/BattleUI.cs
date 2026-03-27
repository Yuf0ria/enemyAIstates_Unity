// ============================================================
//  BattleUI.cs  –  Minimal HUD: status text + death log
//  Attach to a Canvas GameObject in your scene
// ============================================================
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class BattleUI : MonoBehaviour
{
    public static BattleUI Instance { get; private set; }

    [Header("Assign in Inspector")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI logText;

    private Queue<string> logLines = new Queue<string>();
    private const int MAX_LINES = 8;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
    }

    public void LogDeath(string unitName, int team)
    {
        string color = team == 0 ? "cyan" : "red";
        AddLog($"<color={color}>{unitName}</color> has fallen!");
    }

    void AddLog(string line)
    {
        logLines.Enqueue(line);
        if (logLines.Count > MAX_LINES) logLines.Dequeue();
        if (logText != null) logText.text = string.Join("\n", logLines);
    }
}
