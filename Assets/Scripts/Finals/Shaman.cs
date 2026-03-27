// ============================================================
//  Shaman.cs  –  Iron Horde  |  Team 1
//  Trait   : Poison – normal attacks apply damage over time
//  Ability : Mend – heals the most wounded ally
// ============================================================
using System.Collections;
using UnityEngine;

public class Shaman : Unit
{
    [Header("Shaman")]
    public float poisonDps      = 5f;
    public float poisonDuration = 3f;
    public float healAmount     = 40f;

    protected override void PerformAttack(Unit enemy)
    {
        base.PerformAttack(enemy);
        StartCoroutine(PoisonTick(enemy));
    }

    IEnumerator PoisonTick(Unit enemy)
    {
        float elapsed = 0f;
        while (elapsed < poisonDuration && !enemy.isDead)
        {
            enemy.TakeDamage(poisonDps * Time.deltaTime, this);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    protected override void UseAbility()
    {
        Unit wounded = BattleManager.Instance.FindMostWoundedAlly(teamID);
        if (wounded == null) return;
        Debug.Log($"[Shaman] Healing {wounded.unitName}!");
        wounded.HealUnit(healAmount);
        VFXManager.Instance?.FlashAt(wounded.transform.position, Color.green);
    }
}
