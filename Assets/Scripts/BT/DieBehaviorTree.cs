using UnityEngine;

public class DieBehaviorTree : BTBase
{
    private Node rootNode;

    public DieBehaviorTree()
    {
        var sequence = new SequenceNode();
        sequence.AddChild(new TaskDie());
        sequence.AddChild(new TaskDestroySelf());
        rootNode = sequence;
    }

    public override Node.NodeState Evaluate(BlackboardBase blackboard)
    {
        return rootNode.Evaluate(blackboard);
    }
}
