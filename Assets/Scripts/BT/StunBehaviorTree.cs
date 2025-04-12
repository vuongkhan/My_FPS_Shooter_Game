using UnityEngine;
using System.Collections.Generic;

public class StunBehaviorTree : BTBase
{
    private Node rootNode;

    public StunBehaviorTree()
    {
        var selector = new SelectorNode();

        // Sequence: FallDown ➜ StandUp
        var fallDownSequence = new SequenceNode();

        fallDownSequence.AddChild(new TaskFallDown());
        fallDownSequence.AddChild(new TaskStandUp());

        // Decorator để điều kiện damage >= 80
        var fallDownWithCondition = new DecoratorConditions(
            fallDownSequence,
            ConditionMode.AllMustPass,
            bb => bb.TryGet<float>("lastDamage", out var dmg) && dmg >= 80f
        );

        // Nếu không thì chỉ hit nhẹ
        var lightHit = new TaskLightHit();

        selector.AddChild(fallDownWithCondition);
        selector.AddChild(lightHit);

        rootNode = selector;
    }

    public override Node.NodeState Evaluate(BlackboardBase blackboard)
    {
        return rootNode.Evaluate(blackboard);
    }
}
