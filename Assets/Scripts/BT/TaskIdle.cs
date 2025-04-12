using UnityEngine;
public class TaskIdle : Node
{
    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        Debug.Log("idle");
        return NodeState.SUCCESS;
    }
}