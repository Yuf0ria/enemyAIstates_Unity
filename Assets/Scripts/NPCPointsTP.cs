using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcPointsTp : MonoBehaviour
{
    public Transform[] points;
    public bool playerSighted;
    [SerializeField]public NavMeshAgent agent;
    
    private int _destPoint;
    private float _timer,
        _waitAtThePoint = 0.3f,
        _radius = 4;
    

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }
    
    void Update() {
        RadObject(transform.position, _radius);
    }
    
    void RadObject(Vector3 center, float radius)
    {
        OnDrawGizmos();
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log(hitCollider.gameObject.name);
                agent.SetDestination(hitCollider.transform.position);
                playerSighted = true;
            }
            else
            {
                //courutine(PlayerNotInSight)
                StartCoroutine(Wait());
                EnemyLogic_Path();
            }
        }
    }

    private IEnumerator Wait()
    {
        playerSighted = false;
        yield return new WaitForSeconds(1f);
    }

    void EnemyLogic_Path()
    {
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

    void GotoNextPoint() {
        if (points.Length == 0) return;
        _timer += Time.deltaTime;
        if (_timer >= _waitAtThePoint)
        {
            _timer = 0f;
            agent.destination = points[_destPoint].position;
        }
        _destPoint = (_destPoint + 1) % points.Length;
    }
    
    void OnDrawGizmos() //Sphere
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }

    #region _myPreviousCode(for reference)
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
    #endregion

    
    
    
}
