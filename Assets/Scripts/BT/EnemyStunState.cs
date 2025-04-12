using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyStunState : FSMBase
{
    private BTBase stunBT;
    public EnemyStunState(EnemyBase enemy) : base(enemy, StatePriority.Stun) => stunBT = new StunBehaviorTree();
    public override void Enter() => Debug.Log("Enter Stun State");
    public override void Update()
    {
        if (stunBT == null)
        {
            Debug.LogError("stunBT IS NULL!!!");
            return;
        }

        if (enemy == null)
        {
            Debug.LogError("enemy IS NULL!!!");
            return;
        }

        if (enemy.blackboard == null)
        {
            Debug.LogError("enemy.blackboard IS NULL!!!");
            return;
        }

        var result = stunBT.Evaluate(enemy.blackboard);
    }


    public override void Exit() => Debug.Log("Exit Patrol State");
}