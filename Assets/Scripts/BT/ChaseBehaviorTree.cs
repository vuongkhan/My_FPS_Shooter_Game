using UnityEngine;

public class ChaseBehaviorTree : BTBase
{
    private Node rootNode;

    public ChaseBehaviorTree()
    {
        var sequence = new SequenceNode();
        sequence.AddChild(new TaskRotateToPlayer());
        sequence.AddChild(new TaskReactToPlayer());
        sequence.AddChild(new TaskMoveToPlayer());    

        rootNode = sequence;
    }

    public override Node.NodeState Evaluate(BlackboardBase blackboard)
    {
        return rootNode.Evaluate(blackboard);
    }
}
