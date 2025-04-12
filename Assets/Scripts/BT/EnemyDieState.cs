using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDieState : FSMBase
{
    private BTBase dieBT;
    public EnemyDieState(EnemyBase enemy) : base(enemy, StatePriority.Dead) => dieBT = new DieBehaviorTree();
    public override void Enter() => Debug.Log("Enter Die State");
    public override void Update()
    {
        var result = dieBT.Evaluate(enemy.blackboard);
    }


    public override void Exit() => Debug.Log("Exit Die State");
}