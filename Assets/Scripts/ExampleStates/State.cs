using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class State_Zombie : MonoBehaviour
{
    public abstract State_Zombie RunCurrentZombieState();
}

public abstract class State_Human : MonoBehaviour
{
    public abstract State_Human RunCurrentHumanState();
}
