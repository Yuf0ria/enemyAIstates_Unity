// ============================================================
//  Knight.cs  –  Silver Order  |  Team 0
//  Trait   : Sturdy – takes 20% less damage
//  Ability : Shield Bash – disables closest enemy for 1.5s
// ============================================================
using System.Collections;
using UnityEngine;

public class Knight : Unit
{
    [Header("Knight")]
    public float stunDuration = 1.5f;

    // Sturdy: 20% damage reduction
    public override void TakeDamage(float amount, Unit attacker)
        => base.TakeDamage(amount * 0.8f, attacker);

    protected override void UseAbility()
    {
        if (target == null || target.isDead) return;
        Debug.Log($"[Knight] Shield Bash on {target.unitName}!");
        StartCoroutine(Stun(target));
        VFXManager.Instance?.FlashAt(target.transform.position, Color.blue);
    }

    IEnumerator Stun(Unit enemy)
    {
        bool wasDead = enemy.isDead;
        enemy.enabled = false;
        yield return new WaitForSeconds(stunDuration);
        if (!wasDead && !enemy.isDead) enemy.enabled = true;
    }
}
