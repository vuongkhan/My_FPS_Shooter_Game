using System.Collections.Generic;
public class SelectorNode : Node
{
    private List<Node> children = new List<Node>();
    public void AddChild(Node child) => children.Add(child);
    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        foreach (Node child in children)
        {
            NodeState result = child.Evaluate(blackboard);
            if (result == NodeState.SUCCESS) return NodeState.SUCCESS;
            if (result == NodeState.RUNNING) return NodeState.RUNNING;
        }
        return NodeState.FAILURE;
    }
}