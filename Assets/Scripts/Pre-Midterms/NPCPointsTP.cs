#region Assemblies
    using System.Collections;
    using Unity.AI.Navigation;
    using UnityEngine;
    using UnityEngine.AI;
    #endregion

public class NpcPointsTp : MonoBehaviour
{
    #region Components in Inspector
        [Header("Enemy Path")]
        public Transform[] points;
        private bool isWaiting = false,
                     isReRouting = false,
                     isTraversingLink = false;
        [Header("Target Player")]
        public Transform player;
        public bool playerSighted;
        [Header("Enemy Components")]
        [SerializeField]public NavMeshAgent _agent;
        [SerializeField] private LayerMask playerHit; //playerOnSight; raycast
        [SerializeField] private Color color;
        [Header("Height Priority")] [SerializeField]
        private float heightPriorityThreshold = 2f; // high value = enemy
        [SerializeField] private float ScanRadius = 10f; // How far the enemy scans for nearby
    #endregion
    #region Not in Inspector
        private int _destPoint;
        private float _timer,
                      _waitAtThePoint = 6f,
                      _radius = 6, //sphere
                      _distances= 5; //raycast
        private NavMeshLink[] _allLinks;
    #endregion
    private void Start() {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.autoTraverseOffMeshLink = false;
        _allLinks = FindObjectsByType<NavMeshLink>(FindObjectsSortMode.None);
        
        GotoNextPoint();
    }
    
    private void Update() {
        if (_agent.isOnOffMeshLink && !isTraversingLink)
        {
            StartCoroutine(TraverseLink());
            return;
        }

        if (!isTraversingLink)
        {
            RadObject(transform.position, _radius);
            RaycastObj();
        }
    }
    
    #region NavMesh Link Traversal
        private IEnumerator TraverseLink()
        {
            isTraversingLink = true;
            OffMeshLinkData data = _agent.currentOffMeshLinkData;

            Vector3 start = transform.position;
            Vector3 end = data.endPos;
            float elapsed = 0f;

            bool isHigherGround = end.y > start.y + 0.5f; // small threshold to avoid flat links counting
            bool isLowerGround = end.y < start.y - 0.5f;
            bool isJump = data.linkType == OffMeshLinkType.LinkTypeJumpAcross;

            float duration;
            float heightArc;

            if (isHigherGround)
            {
                duration = 0.9f;  // slower going up
                heightArc = 0f;   // no arc, straight climb
            }
            else if (isLowerGround)
            {
                duration = 0.4f;  // faster going down
                heightArc = 1f;   // small arc for drop
            }
            else // lateral jump
            {
                duration = 0.6f;
                heightArc = isJump ? 2f : 0f;
            }

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float height = Mathf.Sin(Mathf.PI * t) * heightArc;
                transform.position = Vector3.Lerp(start, end, t) + Vector3.up * height;
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
            _agent.CompleteOffMeshLink();

            isTraversingLink = false;
        }
    #endregion

    #region Height Priority Logic
        private NavMeshLink FindHighestNearbyLink()
        {
            NavMeshLink bestLink = null;
            float highestY = transform.position.y + 0.5f;

            foreach (NavMeshLink link in _allLinks)
            {
                if (link == null) continue;

                // Check if either endpoint of the link is within scan radius
                Vector3 worldStart = link.transform.TransformPoint(link.startPoint);
                Vector3 worldEnd = link.transform.TransformPoint(link.endPoint);

                bool startInRange = Vector3.Distance(transform.position, worldStart) <= ScanRadius;
                bool endInRange = Vector3.Distance(transform.position, worldEnd) <= ScanRadius;

                if (!startInRange && !endInRange) continue;

                // Prefer the endpoint that is highest
                float linkHighY = Mathf.Max(worldStart.y, worldEnd.y);
                if (linkHighY > highestY)
                {
                    highestY = linkHighY;
                    bestLink = link;
                }
            }

            return bestLink;
        }

        // Returns the world position of the higher endpoint of a given link
        private Vector3 GetHighEndpoint(NavMeshLink link)
        {
            Vector3 worldStart = link.transform.TransformPoint(link.startPoint);
            Vector3 worldEnd = link.transform.TransformPoint(link.endPoint);
            return worldStart.y >= worldEnd.y ? worldStart : worldEnd;
        }

        // Compares the path cost to a target vs path cost through a higher link.
        // Only reroutes if the difference is within threshold (equal-path preference).
        private bool ShouldPreferHigherLink(Vector3 directTarget, NavMeshLink higherLink)
        {
            Vector3 linkEntry = GetHighEndpoint(higherLink);

            NavMeshPath directPath = new NavMeshPath();
            NavMeshPath linkPath = new NavMeshPath();

            _agent.CalculatePath(directTarget, directPath);
            _agent.CalculatePath(linkEntry, linkPath);

            float directCost = GetPathCost(directPath);
            float linkCost = GetPathCost(linkPath);

            // Only prefer the higher link if paths are roughly equal in cost
            return (linkCost - directCost) <= heightPriorityThreshold;
        }

        // Approximates path cost by summing segment lengths
        private float GetPathCost(NavMeshPath path)
        {
            if (path.corners.Length == 0) return float.MaxValue;

            float cost = 0f;
            for (int i = 1; i < path.corners.Length; i++)
                cost += Vector3.Distance(path.corners[i - 1], path.corners[i]);

            return cost;
        }
    #endregion
    
    #region PhysicsObj
        private void RadObject(Vector3 center, float radius)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius);
            playerSighted = false;

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    playerSighted = true;
                    _agent.SetDestination(hitCollider.transform.position);
                    _agent.speed = 8;
                    break;
                }
            }

            if (!playerSighted)
            {
                _agent.speed = 4f;
                if (!isWaiting)
                    StartCoroutine(Wait());
                EnemyLogic_Path();
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
                _agent.SetDestination(player.position);
                playerSighted = true;
            }else Debug.DrawLine(ray.origin, ray.origin + ray.direction * _distances, Color.blue);
        }
    #endregion
    
    #region Logic.EXE :windows noise:
        private void EnemyLogic_Path()
        {
            if (_agent.remainingDistance <= _agent.stoppingDistance && _agent.velocity.sqrMagnitude == 0)
                GotoNextPoint();
            
            if (!isReRouting && (_agent.isPathStale || _agent.pathStatus == NavMeshPathStatus.PathInvalid))
                StartCoroutine(ReRouteEnemyAI());
        }

        private void GotoNextPoint() {
            if (points.Length == 0) return;
            
            _timer += Time.deltaTime;
            if (_timer >= _waitAtThePoint)
            {
                _timer = 0f;
                _agent.destination = points[_destPoint].position;
                _destPoint = (_destPoint + 1) % points.Length;
            }
        }
            
        public void OnDrawGizmos() //Sphere
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }   
    #endregion

    #region IEnumerators
        private IEnumerator Wait()
        {
            isWaiting = true;
            yield return new WaitForSeconds(1f);
            isWaiting = false;
        }
        
        private IEnumerator ReRouteEnemyAI()
        {
            isReRouting = true;
            yield return new WaitForSeconds(15f);
            GotoNextPoint();
            isReRouting = false;
        }
        
    #endregion

    
    
    
}
