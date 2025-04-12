using UnityEngine;
using UnityEngine.AI;

public class TaskMoveToPlayer : Node
{
    private float stoppingBuffer = 1f;
    private float maxSpeed = 5f;
    private float accelerationRate = 0.1f;

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        // Lấy dữ liệu từ Blackboard
        if (!blackboard.TryGet<NavMeshAgent>("agent", out var agent)) return NodeState.FAILURE;
        if (!blackboard.TryGet<GameObject>("target", out var target) || target == null) return NodeState.FAILURE;
        if (!blackboard.TryGet<Animator>("animator", out var animator)) return NodeState.FAILURE;
        if (!agent.isOnNavMesh) return NodeState.FAILURE;

        Vector3 targetPos = target.transform.position;

        // Tăng tốc độ dần nếu có biến speed
        if (blackboard.TryGet<float>("speed", out var speed))
        {
            float newSpeed = Mathf.Min(speed + accelerationRate, maxSpeed);

            if (agent.speed != newSpeed)
            {
                agent.speed = newSpeed;
                blackboard.Set("speed", newSpeed);
                Debug.Log($"🔥 Enemy tăng tốc: {newSpeed}");
            }

            // Đồng bộ với Animator (phải có tham số "Speed" trong Animator Controller)
            animator.SetFloat("Speed", newSpeed);
        }

        // Tránh spam SetDestination
        if (agent.destination != targetPos)
        {
            agent.SetDestination(targetPos);
            agent.isStopped = false;
        }

        // Check đã đến gần player chưa
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + stoppingBuffer)
        {
            agent.isStopped = true;
            agent.ResetPath();

            // Khi dừng lại thì tốc độ anim = 0
            animator.SetFloat("Speed", 0f);
            Debug.Log("✅ Enemy đã tiếp cận player.");
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
