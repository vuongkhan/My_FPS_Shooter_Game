using UnityEngine;

public class TaskReactToPlayer : Node
{
    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        Debug.Log("🧠 [TaskReactToPlayer] Đang kiểm tra Blackboard...");
        return NodeState.SUCCESS;
    }
}
