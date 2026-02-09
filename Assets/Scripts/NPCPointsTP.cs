#region Assemblies
    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
#endregion

public class NpcPointsTp : MonoBehaviour
{
    #region public variables
        public Transform[] points;
        public bool playerSighted;
        [SerializeField]public NavMeshAgent agent;
    #endregion
    #region private variables
        private int _destPoint;
        private float _timer,
            _waitAtThePoint = 6f,
            _radius = 6;
    #endregion
    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }
    
    private void Update() {
        RadObject(transform.position, _radius);
    }
    
    private void RadObject(Vector3 center, float radius)
    {
        //OnDrawGizmos();
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
                StartCoroutine(Wait()); //for player sighted
                EnemyLogic_Path();
            }
        }
    }

    private IEnumerator Wait()
    {
        playerSighted = false;
        yield return new WaitForSeconds(1f);

    }
    
    private IEnumerator ReRouteEnemyAI()
    {
        if (agent.pathPending || agent.remainingDistance > agent.stoppingDistance) yield return new WaitForSeconds(10f); //buffer for the next route
    }

    private void EnemyLogic_Path()
    {
        if (agent.remainingDistance <=  agent.stoppingDistance && agent.velocity.sqrMagnitude == 0)
        {
            GotoNextPoint();
        }

        StartCoroutine(ReRouteEnemyAI());
        
        // Check for obstacles and reroute
        if (agent.isPathStale || agent.pathStatus == NavMeshPathStatus.PathInvalid) {
            // Choose a new point or random point
            GotoNextPoint(); // or implement a random point selection
        }
    }

    private void GotoNextPoint() {
        if (points.Length == 0) 
            // Rerouting logic
            return;
        _timer += Time.deltaTime;
        if (_timer >= _waitAtThePoint)
        {
            _timer = 0f;
            agent.destination = points[_destPoint].position;
        }
        _destPoint = (_destPoint + 1) % points.Length;
    }
    
    public void OnDrawGizmos() //Sphere
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
