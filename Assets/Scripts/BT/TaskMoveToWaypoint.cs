using UnityEngine;
using UnityEngine.AI;

public class TaskMoveToWaypoint : Node
{
    private const float TimeoutDuration = 7f;
    private float timer;

    public override NodeState Evaluate(BlackboardBase bb)
    {
        if (!TryGetData(bb, out var agent, out var animator, out var waypoint))
            return NodeState.FAILURE;

        // Nếu không có đường đi hoặc gần tới thì đặt waypoint
        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypoint.position);
            timer = 0f; // reset timer khi bắt đầu đi
        }

        // Cập nhật animation speed (nếu có animator)
        if (animator != null)
        {
            float speed = agent.velocity.magnitude;
            animator.SetFloat("Speed", speed);
        }

        // Đã tới nơi
        if (!agent.pathPending && agent.remainingDistance < 0.1f)
        {
            AdvanceToNextWaypoint(bb);
            if (animator != null)
                animator.SetFloat("Speed", 0f);
            return NodeState.SUCCESS;
        }

        // Timeout
        if ((timer += Time.deltaTime) >= TimeoutDuration)
        {
            Debug.LogWarning("⏳ Timeout! AI bị kẹt, reset đường đi.");
            agent.ResetPath();
            if (animator != null)
                animator.SetFloat("Speed", 0f);
            return NodeState.FAILURE;
        }

        return NodeState.RUNNING;
    }

    private bool TryGetData(BlackboardBase bb, out NavMeshAgent agent, out Animator animator, out Transform waypoint)
    {
        agent = null;
        animator = null;
        waypoint = null;

        if (!bb.TryGet("agent", out agent)) return false;
        if (!bb.TryGet("currentWaypoint", out waypoint)) return false;
        bb.TryGet("animator", out animator);

        return agent.isOnNavMesh;
    }

    private void AdvanceToNextWaypoint(BlackboardBase bb)
    {
        if (!bb.TryGet<int>("waypointIndex", out int index)) return;
        if (!bb.TryGet<Transform[]>("waypoints", out var waypoints)) return;

        index = (index + 1) % waypoints.Length;

        bb.Set("waypointIndex", index);
        bb.Set("currentWaypoint", waypoints[index]);

        timer = 0f;
    }
}
