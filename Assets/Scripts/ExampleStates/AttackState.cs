using UnityEngine;

public abstract class AttackState_Zombie : State_Zombie
{
    public override State_Zombie RunCurrentZombieState()
    {
        Debug.Log("This State Zombie");
        return this;
    }
}

public abstract class AttackState_Human : State_Human
{
    public override State_Human RunCurrentHumanState()
    {
        Debug.Log("This State Human");
        return this;
    }
}
