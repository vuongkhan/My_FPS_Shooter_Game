using UnityEngine;

public class TaskDie : Node
{
    private const string DieAnim = "Die";
    private const string DieKey = "currentDie";

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<Animator>("animator", out var animator))
        {
            Debug.LogWarning("❌ Không tìm thấy Animator!");
            return NodeState.FAILURE;
        }

        if (!blackboard.TryGet<string>(DieKey, out var current) || current != DieAnim)
        {
            animator.CrossFade(DieAnim, 0.1f);
            blackboard.Set(DieKey, DieAnim);
            Debug.Log("💀 Bắt đầu animation Die.");
            return NodeState.RUNNING;
        }

        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && stateInfo.IsName(DieAnim) && stateInfo.normalizedTime >= 1f)
        {
            blackboard.Remove(DieKey);
            Debug.Log("🪦 Die animation hoàn tất.");
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
