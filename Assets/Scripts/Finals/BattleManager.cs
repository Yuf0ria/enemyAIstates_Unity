
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Team 0 – Silver Order Prefabs")]
    public GameObject knightPrefab;
    public GameObject archerPrefab;
    public GameObject magePrefab;
    public GameObject paladinPrefab;

    [Header("Team 1 – Iron Horde Prefabs")]
    public GameObject berserkerPrefab;
    public GameObject shamanPrefab;
    public GameObject brutePrefab;
    public GameObject hunterPrefab;

    [Header("Spawn Positions (Team 0 left, Team 1 right)")]
    public Transform[] team0Spawns;   // 4 transforms
    public Transform[] team1Spawns;   // 4 transforms

    [Header("Settings")]
    public float battleStartDelay = 2f;


    private List<Unit> team0 = new List<Unit>();
    private List<Unit> team1 = new List<Unit>();
    public bool battleStarted { get; private set; } = false;


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start() => StartCoroutine(BattleRoutine());

    IEnumerator BattleRoutine()
    {
        SpawnAll();
        BattleUI.Instance?.SetStatus("Battle starts in " + battleStartDelay + "s...");
        yield return new WaitForSeconds(battleStartDelay);

        battleStarted = true;
        BattleUI.Instance?.SetStatus("⚔  FIGHT!");

        // Poll for end condition
        yield return new WaitUntil(() => IsBattleOver(out _));
        IsBattleOver(out int winner);

        string msg = winner == 0 ? "Silver Order Wins!" :
                     winner == 1 ? "Iron Horde Wins!"   : "Draw!";
        BattleUI.Instance?.SetStatus(msg);
        Debug.Log("[BattleManager] " + msg);
    }

    //  Spawning
    void SpawnAll()
    {
        Spawn(knightPrefab,    0, team0Spawns, 0);
        Spawn(archerPrefab,    0, team0Spawns, 1);
        Spawn(magePrefab,      0, team0Spawns, 2);
        Spawn(paladinPrefab,   0, team0Spawns, 3);

        Spawn(berserkerPrefab, 1, team1Spawns, 0);
        Spawn(shamanPrefab,    1, team1Spawns, 1);
        Spawn(brutePrefab,     1, team1Spawns, 2);
        Spawn(hunterPrefab,    1, team1Spawns, 3);
    }

    void Spawn(GameObject prefab, int team, Transform[] spawns, int index)
    {
        if (prefab == null || spawns == null || index >= spawns.Length) return;
        var go   = Instantiate(prefab, spawns[index].position, Quaternion.identity);
        var unit = go.GetComponent<Unit>();
        if (unit == null) return;
        unit.teamID = team;
        (team == 0 ? team0 : team1).Add(unit);
    }
    
    public Unit FindClosestEnemy(Unit requester)
    {
        List<Unit> enemies = requester.teamID == 0 ? team1 : team0;
        Unit  closest = null;
        float minDist = float.MaxValue;
        foreach (Unit e in enemies)
        {
            if (e == null || e.isDead) continue;
            float d = Vector3.Distance(requester.transform.position, e.transform.position);
            if (d < minDist) { minDist = d; closest = e; }
        }
        return closest;
    }

    public Unit FindMostWoundedAlly(int teamID)
    {
        List<Unit> allies = teamID == 0 ? team0 : team1;
        Unit  wounded   = null;
        float lowestPct = 1f;
        foreach (Unit a in allies)
        {
            if (a == null || a.isDead) continue;
            if (a.HealthPercent < lowestPct) { lowestPct = a.HealthPercent; wounded = a; }
        }
        return wounded;
    }

    public List<Unit> GetUnitsInRadius(Vector3 center, float radius, int teamID)
    {
        List<Unit> pool   = teamID == 0 ? team0 : team1;
        List<Unit> result = new List<Unit>();
        foreach (Unit u in pool)
        {
            if (u == null || u.isDead) continue;
            if (Vector3.Distance(center, u.transform.position) <= radius)
                result.Add(u);
        }
        return result;
    }

    public List<Unit> GetEnemiesInRadius(Vector3 center, float radius, int attackerTeam)
        => GetUnitsInRadius(center, radius, attackerTeam == 0 ? 1 : 0);


    public void OnUnitDied(Unit unit)
    {
        BattleUI.Instance?.LogDeath(unit.unitName, unit.teamID);
    }

    bool IsBattleOver(out int winner)
    {
        bool t0alive = team0.Exists(u => u != null && !u.isDead);
        bool t1alive = team1.Exists(u => u != null && !u.isDead);

        if (!t0alive && !t1alive) { winner = -1; return true; }
        if (!t0alive)             { winner =  1; return true; }
        if (!t1alive)             { winner =  0; return true; }

        winner = -1;
        return false;
    }
}
