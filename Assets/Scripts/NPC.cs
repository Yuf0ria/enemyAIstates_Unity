using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    [SerializeField] float WaitAtThePoint = 1.5f;
    [SerializeField] NPCPointsTP _path; //calling the script of the points
    
    NavMeshAgent _agent;

    private float timer = 0f;
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _agent.destination =  _path.currentPoint(); //Function
    }

    void Update()
    {
        if (_agent.remainingDistance <= 0.1f) //checks remaining distance
        {
            timer += Time.deltaTime; //start when distance is at 0.1f
            if (timer >= WaitAtThePoint)
            {
                timer = 0f;
                _agent.destination = _path.nextPoint(); //if timer hits 0 it goes to the next point
            }
        }
    }
}
