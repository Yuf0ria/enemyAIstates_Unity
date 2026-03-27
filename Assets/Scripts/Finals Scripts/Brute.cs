// ============================================================
//  Brute.cs  –  Iron Horde  |  Team 1
//  Trait   : Armored – takes 20% less damage
//  Ability : Ground Slam – knocks back all nearby enemies
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : Unit
{
    [Header("Brute")]
    public float knockbackForce    = 6f;
    public float knockbackDuration = 0.3f;
    public float slamRadius        = 4f;

    public override void TakeDamage(float amount, Unit attacker)
        => base.TakeDamage(amount * 0.8f, attacker);

    protected override void UseAbility()
    {
        Debug.Log($"[Brute] Ground Slam!");
        List<Unit> enemies = BattleManager.Instance.GetEnemiesInRadius(
            transform.position, slamRadius, teamID);
        foreach (Unit e in enemies)
        {
            e.TakeDamage(attackDamage, this);
            StartCoroutine(Knockback(e));
        }
        VFXManager.Instance?.FlashAt(transform.position, new Color(0.6f, 0.3f, 0f));
    }

    IEnumerator Knockback(Unit enemy)
    {
        Vector3 dir = (enemy.transform.position - transform.position).normalized;
        float elapsed = 0f;
        bool wasEnabled = enemy.enabled;
        enemy.enabled = false;
        while (elapsed < knockbackDuration)
        {
            enemy.transform.position += dir * knockbackForce * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
        if (!enemy.isDead) enemy.enabled = wasEnabled;
    }
}
