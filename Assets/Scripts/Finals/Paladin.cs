// ============================================================
//  Paladin.cs  –  Silver Order  |  Team 0
//  Trait   : Aura – heals nearby allies every second
//  Ability : Divine Light – burst-heals all nearby allies
// ============================================================
using System.Collections.Generic;
using UnityEngine;

public class Paladin : Unit
{
    [Header("Paladin")]
    public float auraHeal   = 3f;
    public float auraRadius = 5f;
    public float burstHeal  = 30f;

    private float auraTimer = 0f;

    protected override void Update()
    {
        base.Update();
        if (isDead) return;

        auraTimer += Time.deltaTime;
        if (auraTimer >= 1f)
        {
            auraTimer = 0f;
            foreach (Unit ally in BattleManager.Instance.GetUnitsInRadius(
                transform.position, auraRadius, teamID))
                ally.HealUnit(auraHeal);
        }
    }

    protected override void UseAbility()
    {
        Debug.Log($"[Paladin] Divine Light!");
        foreach (Unit ally in BattleManager.Instance.GetUnitsInRadius(
            transform.position, auraRadius * 1.5f, teamID))
            ally.HealUnit(burstHeal);
        VFXManager.Instance?.FlashAt(transform.position, Color.yellow);
    }
}
