#region Assemblies
    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
#endregion

public class NpcPointsTp : MonoBehaviour
{
    #region public variables
        public Transform[] points;
        public Transform player;
        public bool playerSighted,
                    isItRunning;
        [SerializeField]public NavMeshAgent agent;
    #endregion
    #region private variables
        private int _destPoint;
        private float _timer,
            _waitAtThePoint = 6f,
            _radius = 6, //sphere
            _distances = 10; //raycast
        [SerializeField] private LayerMask playerHit; //playerOnSight; raycast
        [SerializeField] private Color color;
    #endregion
    private void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
    }
    
    private void Update() {
        RadObject(transform.position, _radius);
        RaycastObj();
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
    
    private void RaycastObj()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, _distances, playerHit))
        {
            Debug.Log($"Object name {hitInfo.transform.name}");
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            agent.SetDestination(player.position);
            playerSighted = true;
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * _distances, Color.blue);
            //wait or stop
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

    private IEnumerator Look_Patrol(Transform self)
    {
        if (10 >= Random.Range(1, 500))
        {
            Debug.Log("it runs");
        
            // Cache the start, left, and right extremes of our rotation.
            Quaternion start = self.rotation;
            Quaternion left = start * Quaternion.Euler(0, -45, 0);
            Quaternion right = start * Quaternion.Euler(0, 45, 0);

            // Yield control to the Rotate coroutine to execute
            // each turn in sequence, and resume here after each
            // invocation of Rotate finishes its work.

            yield return Rotate(self, start, left, 1.0f);

            yield return Rotate(self, left, right, 2.0f);

            yield return Rotate(self, right, start, 1.0f);
        }
        isItRunning = true;
    }
    
    IEnumerator Rotate(Transform self, Quaternion from, Quaternion to, float duration) {

        for (float t = 0; t < 1f; t += Time.deltaTime / duration) {
            // Rotate to match our current progress between from and to.
            self.rotation = Quaternion.Slerp(from, to, t);
            // Wait one frame before looping again.
            yield return null;
        }

        // Ensure we finish exactly at the destination orientation.
        self.rotation = to;
    }

    private void EnemyLogic_Path()
    {
        if (agent.remainingDistance <=  agent.stoppingDistance && agent.velocity.sqrMagnitude == 0)
        {
            //Look_Patrol ;IEnumerator
            StartCoroutine(Look_Patrol(transform));
            GotoNextPoint();
        }

        StartCoroutine(ReRouteEnemyAI());
        
        // Check for obstacles and reroute
        if (agent.isPathStale || agent.pathStatus == NavMeshPathStatus.PathInvalid) {
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
