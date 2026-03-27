// ============================================================
//  Hunter.cs  –  Iron Horde  |  Team 1
//  Trait   : Kite – retreats when enemy is too close
//  Ability : Mark – target takes +50% damage for 4 seconds
// ============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Unit
{
    [Header("Hunter")]
    public float markDuration    = 4f;
    public float markDamageMulti = 1.5f;

    static HashSet<Unit> markedUnits = new HashSet<Unit>();

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

    protected override void PerformAttack(Unit enemy)
    {
        float bonus = markedUnits.Contains(enemy) ? markDamageMulti : 1f;
        enemy.TakeDamage(attackDamage * bonus, this);
        VFXManager.Instance?.FlashAt(enemy.transform.position, Color.red);
    }

    protected override void UseAbility()
    {
        if (target == null || target.isDead) return;
        Debug.Log($"[Hunter] Marked {target.unitName}!");
        StartCoroutine(ApplyMark(target));
        VFXManager.Instance?.FlashAt(target.transform.position, new Color(1f, 0.4f, 0f));
    }

    IEnumerator ApplyMark(Unit enemy)
    {
        markedUnits.Add(enemy);
        yield return new WaitForSeconds(markDuration);
        markedUnits.Remove(enemy);
    }
}
