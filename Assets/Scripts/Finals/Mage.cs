// ============================================================
//  Mage.cs  –  Silver Order  |  Team 0
//  Trait   : Fragile – takes 20% more damage
//  Ability : Blizzard – slows + damages all enemies in radius
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Unit
{
    [Header("Mage")]
    public float blizzardRadius = 5f;
    public float slowDuration   = 2f;
    public float slowAmount     = 0.5f;

    public override void TakeDamage(float amount, Unit attacker)
        => base.TakeDamage(amount * 1.2f, attacker);

    protected override void UseAbility()
    {
        if (target == null) return;
        Debug.Log($"[Mage] Blizzard!");
        List<Unit> enemies = BattleManager.Instance.GetEnemiesInRadius(
            target.transform.position, blizzardRadius, teamID);
        foreach (Unit e in enemies)
        {
            e.TakeDamage(attackDamage * 1.5f, this);
            StartCoroutine(ApplySlow(e));
        }
        VFXManager.Instance?.FlashAt(target.transform.position, Color.white);
    }

    IEnumerator ApplySlow(Unit enemy)
    {
        float original = enemy.moveSpeed;
        enemy.moveSpeed *= (1f - slowAmount);
        yield return new WaitForSeconds(slowDuration);
        if (!enemy.isDead) enemy.moveSpeed = original;
    }
}
