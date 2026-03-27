
public abstract class ChaseState_Zombie : State_Zombie
{
    public AttackState_Zombie AttackStateZombie;
    public bool isInAttackRange;
    
    public override State_Zombie RunCurrentZombieState()
    {
        if (isInAttackRange)
        {
            return AttackStateZombie;
        }
        else
        {
            return this;
        }
    }
}

public abstract class ChaseState_Human : State_Human
{
    public AttackState_Human AttackStateHuman;
    public bool isInAttackRange;
    
    public override State_Human RunCurrentHumanState()
    {
        if (isInAttackRange)
        {
            return AttackStateHuman;
        }
        else
        {
            return this;
        }
    }
}
