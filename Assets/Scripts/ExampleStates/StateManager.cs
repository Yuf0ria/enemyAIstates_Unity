using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour
{
    public State_Human currentState_h;
    public State_Zombie currentState_z;
    void Update()
    {
        RunStateMachine_Zombie();
        RunStateMachine_Human();
    }
    
    //Zombie State
    private void RunStateMachine_Zombie()
    {
        State_Zombie nextState_z =  currentState_z?.RunCurrentZombieState();

        if (nextState_z != null)
        {
            SwitchtoTheNextState(nextState_z);
        }
    }

    private void SwitchtoTheNextState(State_Zombie nextState)
    {
        currentState_z = nextState;
    }
    
    //Human State
    private void RunStateMachine_Human()
    {
        State_Human nextState =  currentState_h?.RunCurrentHumanState();

        if (nextState != null)
        {
            SwitchtoTheNextState(nextState);
        }
    }

    private void SwitchtoTheNextState(State_Human nextState)
    {
        currentState_h = nextState;
    }
}
