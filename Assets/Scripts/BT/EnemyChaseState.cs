using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyChaseState : FSMBase
{
    private BTBase chaseBT;
    public EnemyChaseState(EnemyBase enemy) : base(enemy, StatePriority.Chase) => chaseBT = new ChaseBehaviorTree();
    public override void Enter() => Debug.Log("Enter Chase State");
    public override void Update()
    {
        var result = chaseBT.Evaluate(enemy.blackboard);

        // Nếu đã tới gần player → chuyển sang AttackState
        if (result == Node.NodeState.SUCCESS)
        {
            enemy.fsmController.ChangeState(new EnemyMeleeAttackState(enemy));
            return;
        }
    }
    public override void Exit()
    {
        Debug.Log("Exit Chase State");

        if (enemy.TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
        {
            agent.ResetPath();          
            agent.isStopped = true;     
        }
    }
}