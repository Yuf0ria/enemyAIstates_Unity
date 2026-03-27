// ============================================================
//  Berserker.cs  –  Iron Horde  |  Team 1
//  Trait   : Enrage – attacks faster below 40% HP
//  Ability : Blood Rage – damages nearby enemies (costs HP)
// ============================================================
using System.Collections.Generic;
using UnityEngine;

public class Berserker : Unit
{
    [Header("Berserker")]
    public float rageThreshold = 0.4f;
    public float selfDamage    = 15f;
    public float rageRadius    = 3f;

    private bool enraged = false;

    public override void TakeDamage(float amount, Unit attacker)
    {
        base.TakeDamage(amount, attacker);
        if (!enraged && HealthPercent <= rageThreshold)
        {
            enraged = true;
            attackCooldown *= 0.6f;
            Debug.Log($"[Berserker] ENRAGED!");
        }
    }

    protected override void UseAbility()
    {
        Debug.Log($"[Berserker] Blood Rage!");
        List<Unit> enemies = BattleManager.Instance.GetEnemiesInRadius(
            transform.position, rageRadius, teamID);
        foreach (Unit e in enemies)
            e.TakeDamage(attackDamage * 1.5f, this);
        TakeDamage(selfDamage, this);
        VFXManager.Instance?.FlashAt(transform.position, Color.red);
    }
}
