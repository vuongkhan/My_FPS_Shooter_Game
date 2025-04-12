using UnityEngine;

public class TaskFallDown : Node
{
    private const string FallAnimName = "FallBack";
    private const string CurrentAnimKey = "currentFall";

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<Animator>("animator", out var animator))
        {
            Debug.LogWarning("❌ Không tìm thấy Animator trong blackboard!");
            return NodeState.FAILURE;
        }

        // Nếu chưa đang ngã, bắt đầu animation
        if (!blackboard.TryGet<string>(CurrentAnimKey, out var current) || current != FallAnimName)
        {
            animator.CrossFade(FallAnimName, 0.1f);
            blackboard.Set(CurrentAnimKey, FallAnimName);
            Debug.Log("😵 Bắt đầu FallBack animation");
            return NodeState.RUNNING;
        }

        // Kiểm tra animation kết thúc chưa
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && stateInfo.IsName(FallAnimName) && stateInfo.normalizedTime >= 1f)
        {
            animator.Play("Idle");
            blackboard.Remove(CurrentAnimKey);
            Debug.Log("✅ Đã ngã xong, chuyển về Idle");

            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
