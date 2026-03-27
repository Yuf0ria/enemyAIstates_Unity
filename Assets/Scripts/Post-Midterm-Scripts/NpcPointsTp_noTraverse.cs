//Version nung pero walang jumping kasi masdyadong masakit sa ulo
//This makes the npc walk tot he points
//added .speed cause they slow originally
#region Assemblies
    using System.Collections;
    using UnityEngine;
    using UnityEngine.AI;
#endregion

public class NpcPointsTp_noTraverse : MonoBehaviour
{
    #region Components in Inspector
        [Header("NPC Path")]
        [SerializeField] public Transform[] points;
        [Header("NPC Stuff")]
        [SerializeField] public NavMeshAgent _agent;
    #endregion
    #region Not in Inspector
        private bool isWaiting = false;
        private bool isReRouting = false;
        private int _destPoint;
        private float _timer;
        private float _waitAtThePoint = 2f;
        private float _radius = 6f;
    #endregion
    
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.autoBraking = false;
        _agent.speed = 10f;
        GotoNextPoint();
    }
    
    private void Update()
    {
        NPCPath();
    }
    
    #region Logic.EXE
        private void NPCPath()
        {
            // checks npc distance
            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    if (!isWaiting)
                    {
                        StartCoroutine(WaitAtPoint());
                    }
                }
            }
            
            // if di-madaanan yung path
            if (!isReRouting && (_agent.isPathStale || _agent.pathStatus == NavMeshPathStatus.PathInvalid))
            {
                StartCoroutine(ReRouteNPC());
            }
        }

        private void GotoNextPoint()
        {
            if (points.Length == 0) return;
            
            _agent.destination = points[_destPoint].position;
            _destPoint = (_destPoint + 1) % points.Length;
        }
            
        public void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }   
    #endregion

    #region IEnumerators
        private IEnumerator WaitAtPoint()
        {
            isWaiting = true;
            yield return new WaitForSeconds(_waitAtThePoint);
            GotoNextPoint();
            isWaiting = false;
        }
        
        private IEnumerator ReRouteNPC()
        {
            isReRouting = true;
            yield return new WaitForSeconds(2f);
            GotoNextPoint();
            isReRouting = false;
        }
    #endregion
}