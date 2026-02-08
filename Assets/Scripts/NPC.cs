using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    // [SerializeField] float WaitAtThePoint = 1.5f;
    // [SerializeField] NpcPointsTp _path; //calling the script of the points
    //
    // [SerializeField]NavMeshAgent _agent;
    //
    // private float timer = 0f;
    // void Awake()
    // {
    //     _agent = GetComponent<NavMeshAgent>();
    //     
    // }
    //
    // void Start()
    // {
    //     _path.NextPoint();
    //     _agent.autoBraking = false;
    // }
    //
    // void Update()
    // {
    //     if (_agent.remainingDistance <= 0.1f) //checks remaining distance
    //     {
    //         timer += Time.deltaTime; //start when distance is at 0.1f
    //         if (timer >= WaitAtThePoint)
    //         {
    //             timer = 0f;
    //             _path.NextPoint(); //if timer hits 0 it goes to the next point
    //         }
    //     }
    //     
    //     // Rerouting logic
    //     if (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance) return;
    //
    //     // Check for obstacles and reroute
    //     if (_agent.isPathStale || _agent.pathStatus == NavMeshPathStatus.PathInvalid) {
    //         // Choose a new point or random point
    //         _path.NextPoint(); // or implement a random point selection
    //     }
    //     
    //     // Check for obstacles
    //     if (Physics.Raycast(transform.position, _agent.velocity.normalized, out RaycastHit hit, 1f)) {
    //         if (hit.collider.CompareTag("Obstacle")) {
    //             // Reroute to the next point
    //             _path.NextPoint();
    //         }
    //     }
    // }
    
    
}
