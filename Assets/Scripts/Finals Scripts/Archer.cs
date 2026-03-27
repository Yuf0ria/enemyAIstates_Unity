// ============================================================
//  Archer.cs  –  Silver Order  |  Team 0
//  Trait   : Kite – retreats when enemy is too close
//  Ability : Volley – damages all enemies in a wide radius
// ============================================================
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    [Header("Archer")]
    public float volleyRadius = 8f;

    protected override void OnChaseState()
    {
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist < attackRange * 0.6f)
            {
                Vector3 away = (transform.position - target.transform.position).normalized;
                transform.position += away * moveSpeed * Time.deltaTime;
                return;
            }
        }
        base.OnChaseState();
    }

    protected override void UseAbility()
    {
        Debug.Log($"[Archer] Volley!");
        List<Unit> enemies = BattleManager.Instance.GetEnemiesInRadius(
            transform.position, volleyRadius, teamID);
        foreach (Unit e in enemies)
            e.TakeDamage(attackDamage * 0.6f, this);
        VFXManager.Instance?.FlashAt(transform.position, Color.cyan);
    }
}
