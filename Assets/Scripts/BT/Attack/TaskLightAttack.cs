using UnityEngine;

public class TaskLightAttack : Node
{
    private const string AttackKey = "LightAttack"; // Tên state trong Animator
    private const string CurrentAttackKey = "currentAttack";

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<Animator>("animator", out var animator))
        {
            Debug.LogWarning("❌ Không tìm thấy Animator trong blackboard!");
            return NodeState.FAILURE;
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (!blackboard.TryGet<string>(CurrentAttackKey, out var currentAttack) || currentAttack != AttackKey)
        {
            if (stateInfo.IsName(AttackKey))
            {
                animator.Play("Idle", 0, 0); 
                animator.Update(0);          
            }

            animator.CrossFade(AttackKey, 0.1f);
            blackboard.Set(CurrentAttackKey, AttackKey);

            Debug.Log("⚔️ Light Attack bắt đầu!");
            return NodeState.RUNNING;
        }
        if (!animator.IsInTransition(0) && stateInfo.IsName(AttackKey) && stateInfo.normalizedTime >= 0.95f)
        {
            animator.Play("Idle");
            blackboard.Remove(CurrentAttackKey);
            Debug.Log("✅ Light Attack hoàn tất.");
            if (blackboard.TryGet<EnemyBase>("enemy", out var enemy))
            {
                enemy.fsmController.ForceChangeState(new EnemyChaseState(enemy));
                Debug.Log("🏃‍♂️ Chuyển sang EnemyChaseState sau khi LightAttack.");
            }

            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
