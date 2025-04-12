using UnityEngine;

public class TaskLookAround : Node
{
    private const string TimerKey = "__lookAroundTimer";
    private const string LookingKey = "__lookAroundActive";
    private float lookTime = 10f;

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<GameObject>("owner", out var ownerGO)) return NodeState.FAILURE;
        if (!ownerGO.TryGetComponent<Animator>(out var animator)) return NodeState.FAILURE;

        blackboard.TryGet<float>(TimerKey, out var timer);
        blackboard.TryGet<bool>(LookingKey, out var isLooking);

        if (!isLooking)
        {
            animator.SetTrigger("LookAround");
            blackboard.Set(LookingKey, true);
            Debug.Log("👀 AI bắt đầu nhìn quanh...");
        }

        timer += Time.deltaTime;
        blackboard.Set(TimerKey, timer);

        if (timer >= lookTime)
        {
            blackboard.Set(TimerKey, 0f);
            blackboard.Set(LookingKey, false);
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
