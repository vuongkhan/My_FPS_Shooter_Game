using UnityEngine;

public class TaskLightHit : Node
{
    private const string HitAnimName = "LightHit";
    private const string CurrentAnimKey = "currentHit";

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<Animator>("animator", out var animator))
        {
            Debug.LogWarning("❌ Không tìm thấy Animator trong blackboard!");
            return NodeState.FAILURE;
        }
        if (!blackboard.TryGet<string>(CurrentAnimKey, out var current) || current != HitAnimName)
        {
            animator.CrossFade(HitAnimName, 0.05f);
            blackboard.Set(CurrentAnimKey, HitAnimName);
            Debug.Log("👊 Bắt đầu LightHit animation");
            return NodeState.RUNNING;
        }
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!animator.IsInTransition(0) && stateInfo.IsName(HitAnimName) && stateInfo.normalizedTime >= 1f)
        {
            animator.Play("Idle");
            blackboard.Remove(CurrentAnimKey);
            Debug.Log("👌 Đã phản ứng xong, quay về Idle");
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
