//There Two States Here

//Zombie Part Here
#region Zombie Idle State
    public abstract class IdleState_Zombie : State_Zombie
    {
        public ChaseState_Zombie chaseState;
        public bool canSeeTheHuman;
        
        public override State_Zombie RunCurrentZombieState()
        {
            if (canSeeTheHuman)
            {
                return chaseState;
            }
            else
            {
                return this;
            }
        }
    }
#endregion

//Human Part Here
#region Human Idle State
    public abstract class IdleState_Human : State_Human
    {
        public ChaseState_Human chaseState_Human;
        public bool canSeeTheZombie;
        
        public override State_Human RunCurrentHumanState()
        {
            if (canSeeTheZombie)
            {
                return chaseState_Human;
            }
            else
            {
                return this;
            }
            
        }
    }
#endregion


