using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;

/// <summary>
/// Simple Behavior Tree + NavMesh AI.
/// Priority:
/// 1) Chase target
/// 2) Patrol waypoints
/// 3) Idle
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class StateNode : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Transform[] patrolPoints;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float loseRange = 14f;

    [Header("Patrol")]
    public float waypointTolerance = 1.0f;
    public float idleAtWaypointSeconds = 1.0f;

    private NavMeshAgent agent;

    private enum NodeState { Success, Failure, Running }

    private abstract class Node
    {
        public abstract NodeState Tick();
    }

    // Tries children in order until one succeeds or is still running
    private class Selector : Node
    {
        private readonly List<Node> children;
        public Selector(List<Node> children) => this.children = children;

        public override NodeState Tick()
        {
            foreach (var child in children)
            {
                var state = child.Tick();
                if (state == NodeState.Success) return NodeState.Success;
                if (state == NodeState.Running) return NodeState.Running;
            }
            return NodeState.Failure;
        }
    }

    // Runs children in order; fails if any step fails
    private class Sequence : Node
    {
        private readonly List<Node> children;
        public Sequence(List<Node> children) => this.children = children;

        public override NodeState Tick()
        {
            foreach (var child in children)
            {
                var state = child.Tick();
                if (state == NodeState.Failure) return NodeState.Failure;
                if (state == NodeState.Running) return NodeState.Running;
            }
            return NodeState.Success;
        }
    }

    // Wraps a method as a behavior tree leaf node
    private class ActionNode : Node
    {
        private readonly Func<NodeState> action;
        public ActionNode(Func<NodeState> action) => this.action = action;
        public override NodeState Tick() => action();
    }

    private bool isChasing;
    private int patrolIndex;
    private float idleTimer;

    private Node root;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Root: Chase -> Patrol -> Idle
        var chaseSequence = new Sequence(new List<Node>
        {
            new ActionNode(IsTargetDetected),
            new ActionNode(ChaseTarget)
        });

        var patrolSequence = new Sequence(new List<Node>
        {
            new ActionNode(HasPatrolPoints),
            new ActionNode(Patrol)
        });

        var idleAction = new ActionNode(Idle);

        root = new Selector(new List<Node>
        {
            chaseSequence,
            patrolSequence,
            idleAction
        });
    }

    private void Update()
    {
        root.Tick();
    }

    // Starts chase when inside detectionRange, keeps chasing until beyond loseRange
    private NodeState IsTargetDetected()
    {
        if (target == null) return NodeState.Failure;

        float d = Vector3.Distance(transform.position, target.position);

        if (!isChasing)
        {
            if (d <= detectionRange)
            {
                isChasing = true;
                return NodeState.Success;
            }
            return NodeState.Failure;
        }
        else
        {
            if (d <= loseRange) return NodeState.Success;

            isChasing = false;
            return NodeState.Failure;
        }
    }

    private NodeState ChaseTarget()
    {
        if (target == null) return NodeState.Failure;

        agent.isStopped = false;
        agent.SetDestination(target.position);
        return NodeState.Running;
    }

    private NodeState HasPatrolPoints()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return NodeState.Failure;
        return NodeState.Success;
    }

    // Moves through patrol points and pauses briefly at each one
    private NodeState Patrol()
    {
        if (isChasing) return NodeState.Failure;

        Transform current = patrolPoints[patrolIndex];
        if (current == null) return NodeState.Failure;

        if (idleTimer > 0f)
        {
            agent.isStopped = true;
            idleTimer -= Time.deltaTime;
            return NodeState.Running;
        }

        agent.isStopped = false;
        agent.SetDestination(current.position);

        if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
        {
            idleTimer = idleAtWaypointSeconds;
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        return NodeState.Running;
    }

    private NodeState Idle()
    {
        agent.isStopped = true;
        return NodeState.Running;
    }

    // Visualize detection and lose ranges in the Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }
}
