using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyMeleeAttackState : FSMBase
{
    private BTBase attackBT;
    public EnemyMeleeAttackState(EnemyBase enemy) : base(enemy, StatePriority.Attack) => attackBT = new MeleeAttackBehaviorTree();
    public override void Enter() => Debug.Log("Enter Patrol State");
    public override void Update()
    {
        var result = attackBT.Evaluate(enemy.blackboard);

        Debug.Log("Attack");

    }

    public override void Exit() => Debug.Log("Exit Attack State");
}