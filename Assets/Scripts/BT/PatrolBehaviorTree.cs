using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PatrolBehaviorTree : BTBase
{
    private Node rootNode;
    public PatrolBehaviorTree()
    {
        var sequence = new SequenceNode();
        sequence.AddChild(new TaskMoveToWaypoint());
        sequence.AddChild(new TaskIdle());
        sequence.AddChild(new TaskLookAround());
        rootNode = sequence;
    }
    public override Node.NodeState Evaluate(BlackboardBase blackboard) => rootNode.Evaluate(blackboard);
}