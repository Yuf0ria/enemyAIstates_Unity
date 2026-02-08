using UnityEngine;
using UnityEngine.AI;

public class NpcPointsTp : MonoBehaviour
{
    // #region public variables
    //     public Transform[] points; //Array for the points
    // #endregion
    // #region private variables
    //     private int _direction = 0;
    //     [SerializeField]private NavMeshAgent Enemy;
    // #endregion
    //
    // //nextPoint()
    // public void NextPoint()
    // {
    //     if(points.Length == 0) return;
    //     Enemy.destination = points[_direction].position;
    //     _direction = (_direction + 1) % points.Length;
    // }
    public Transform[] points;
    [SerializeField]public NavMeshAgent agent;
    private int destPoint = 0;
    private float timer;
    private float WaitAtThePoint = 0.3f;
    

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }

    void GotoNextPoint() {
        if (points.Length == 0) return;
        timer += Time.deltaTime;
        if (timer >= WaitAtThePoint)
        {
            timer = 0f;
            agent.destination = points[destPoint].position;
        }
        destPoint = (destPoint + 1) % points.Length;
    }

    void Update() {
        if (agent.remainingDistance <=  agent.stoppingDistance && agent.velocity.sqrMagnitude == 0)
        {
            GotoNextPoint();
        }
        
        // Rerouting logic
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) return; 

        // Check for obstacles and reroute
        if (agent.isPathStale || agent.pathStatus == NavMeshPathStatus.PathInvalid) {
            // Choose a new point or random point
            GotoNextPoint(); // or implement a random point selection
        }

    }
    
    
}
