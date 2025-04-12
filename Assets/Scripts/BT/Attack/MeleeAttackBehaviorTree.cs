using UnityEngine;
using System.Collections.Generic;

public class MeleeAttackBehaviorTree : BTBase
{
    private Node rootNode;

    public MeleeAttackBehaviorTree()
    {
        var selector = new SelectorNode();
        var heavyAttackCondition = new DecoratorConditions(
            new TaskHeavyAttack(),
            ConditionMode.AllMustPass,
            blackboard => blackboard.TryGet<float>("stamina", out var stamina) && stamina >= 60
        );
        var lightAttack = new TaskLightAttack();
       selector.AddChild(heavyAttackCondition);
        selector.AddChild(lightAttack);

        rootNode = selector;
        
    }

    public override Node.NodeState Evaluate(BlackboardBase blackboard)
    {
        return rootNode.Evaluate(blackboard);
    }
}
