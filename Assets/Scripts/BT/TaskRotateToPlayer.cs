using UnityEngine;

public class TaskRotateToPlayer : Node
{
    private float rotateSpeed = 5f;
    private float delayTime = 2f;
    private float startTime = -1f;

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<GameObject>("owner", out var ownerGO)) return NodeState.FAILURE;
        if (!blackboard.TryGet<GameObject>("target", out var target) || target == null) return NodeState.FAILURE;

        Transform self = ownerGO.transform;
        Vector3 dir = (target.transform.position - self.position).normalized;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            self.rotation = Quaternion.RotateTowards(self.rotation, targetRotation, rotateSpeed * Time.deltaTime * 100f);
        }

        // Bắt đầu đếm thời gian xoay
        if (startTime < 0f)
            startTime = Time.time;

        if (Time.time - startTime >= delayTime)
        {
            startTime = -1f;
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
