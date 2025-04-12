using UnityEngine;

public class TaskStandUp : Node
{
    private const string StandUpAnimName = "StandUp";
    private const string CurrentAnimKey = "currentFall"; // Giữ key này để đồng bộ flow

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<Animator>("animator", out var animator))
        {
            Debug.LogWarning("❌ Không tìm thấy Animator trong blackboard!");
            return NodeState.FAILURE;
        }

        // Nếu chưa bắt đầu đứng dậy thì bắt đầu animation
        if (!blackboard.TryGet<string>(CurrentAnimKey, out var current) || current != StandUpAnimName)
        {
            animator.CrossFade(StandUpAnimName, 0.1f);
            blackboard.Set(CurrentAnimKey, StandUpAnimName);
            Debug.Log("🧍‍♂️ Bắt đầu StandUp animation");
            return NodeState.RUNNING;
        }

        // Kiểm tra animation kết thúc chưa
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && stateInfo.IsName(StandUpAnimName) && stateInfo.normalizedTime >= 1f)
        {
            blackboard.Remove(CurrentAnimKey);
            animator.Play("Idle");
            Debug.Log("✅ Đã đứng dậy xong.");
            // 👉 Gọi FSM chuyển sang Chase state
            if (blackboard.TryGet<EnemyBase>("enemy", out var enemy))
            {
                enemy.fsmController.ForceChangeState(new EnemyChaseState(enemy));
                Debug.Log("🏃‍♂️ Đã chuyển sang EnemyChaseState sau khi FallDown.");
            }
            else
            {
                Debug.LogWarning("❌ Không tìm thấy 'enemy' trong blackboard để đổi state.");
            }
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
