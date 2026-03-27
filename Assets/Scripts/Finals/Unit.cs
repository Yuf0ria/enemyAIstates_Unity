//  Base class for all units
using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Stats")]
    public string unitName   = "Unit";
    public int    teamID     = 0;
    public float  maxHealth  = 100f;
    public float  moveSpeed  = 3f;
    public float  attackRange = 2f;
    public float  attackDamage = 10f;
    public float  attackCooldown = 1f;   // seconds between attacks

    [Header("Ability")]
    public float  abilityCooldown = 8f;

    [HideInInspector] public float  currentHealth;
    [HideInInspector] public bool   isDead = false;

    public enum State { Idle, Chase, Attack }
    [Header("Debug")]
    public State currentState = State.Idle;

    protected float attackTimer  = 0f;
    protected float abilityTimer = 0f;
    protected Unit  target;

    protected HealthBar healthBar;
    protected Renderer  bodyRenderer;

    public System.Action<Unit> OnDeath;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        healthBar     = GetComponentInChildren<HealthBar>();
        bodyRenderer  = GetComponentInChildren<Renderer>();
    }

    protected virtual void Start()
    {
        healthBar?.Initialize(maxHealth, unitName);
    }

    protected virtual void Update()
    {
        if (isDead) return;

        attackTimer  += Time.deltaTime;
        abilityTimer += Time.deltaTime;

        RunFSM();
    }

    void RunFSM()
    {
        // Always refresh target
        if (target == null || target.isDead)
            target = BattleManager.Instance?.FindClosestEnemy(this);

        if (target == null)
        {
            currentState = State.Idle;
            return;
        }

        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (dist <= attackRange)
        {
            currentState = State.Attack;
            OnAttackState();
        }
        else
        {
            currentState = State.Chase;
            OnChaseState();
        }
    }
    
    protected virtual void OnChaseState()
    {
        MoveToward(target.transform.position);

        // Ability can fire while chasing if in range
        if (abilityTimer >= abilityCooldown)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (CanUseAbility(dist))
            {
                UseAbility();
                abilityTimer = 0f;
            }
        }
    }

    protected virtual void OnAttackState()
    {
        FaceTarget(target.transform.position);

        // Ability takes priority
        if (abilityTimer >= abilityCooldown && CanUseAbility(
                Vector3.Distance(transform.position, target.transform.position)))
        {
            UseAbility();
            abilityTimer = 0f;
        }
        else if (attackTimer >= attackCooldown)
        {
            PerformAttack(target);
            attackTimer = 0f;
        }
    }

    //Movement
    protected void MoveToward(Vector3 destination)
    {
        Vector3 dir = (destination - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        FaceTarget(destination);
    }

    protected void FaceTarget(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        dir.y = 0f;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(
                transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
    }

    //Fighting
    protected virtual void PerformAttack(Unit enemy)
    {
        enemy.TakeDamage(attackDamage, this);
        VFXManager.Instance?.FlashAt(enemy.transform.position, Color.red);
    }

    public virtual void TakeDamage(float amount, Unit attacker)
    {
        if (isDead) return;
        currentHealth -= amount;
        healthBar?.UpdateHealth(currentHealth);
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0f) Die();
    }

    protected virtual void Die()
    {
        isDead = true;
        currentState = State.Idle;
        OnDeath?.Invoke(this);
        BattleManager.Instance?.OnUnitDied(this);
        StartCoroutine(DeathCleanup());
    }

    IEnumerator DeathCleanup()
    {
        // Tilt and shrink on death
        float t = 0f;
        Vector3 startScale = transform.localScale;
        while (t < 1f)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator DamageFlash()
    {
        if (bodyRenderer != null) bodyRenderer.material.color = Color.white;
        yield return new WaitForSeconds(0.08f);
        ApplyColor(GetTeamColor());
    }


    protected virtual void UseAbility() { }

    // Return true if distance is within a valid ability range
    protected virtual bool CanUseAbility(float dist) => dist <= attackRange * 3f;


    public void HealUnit(float amount)
    {
        if (isDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar?.UpdateHealth(currentHealth);
        VFXManager.Instance?.FlashAt(transform.position, Color.green);
    }

    protected void ApplyColor(Color c)
    {
        if (bodyRenderer != null) bodyRenderer.material.color = c;
    }

    protected virtual Color GetTeamColor() => teamID == 0 ? Color.red : Color.blue;

    public float HealthPercent => currentHealth / maxHealth;
}
