using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrolState : FSMBase
{
    private BTBase patrolBT;

    public EnemyPatrolState(EnemyBase enemy)
        : base(enemy, StatePriority.Patrol)
    {
        patrolBT = new PatrolBehaviorTree();
    }

    public override void Enter()
    {
        Debug.Log("Enter Patrol State");
    }

    public override void Update()
    {
        var result = patrolBT.Evaluate(enemy.blackboard);

        // Nếu thấy địch → chuyển sang chase
        if (enemy.blackboard.Get<bool>("canSeeEnemy"))
        {
            enemy.fsmController.ChangeState(new EnemyChaseState(enemy));
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Patrol State");

        if (enemy.TryGetComponent<NavMeshAgent>(out var agent))
        {
            agent.ResetPath();           // 🛑 Dừng di chuyển ngay
            agent.isStopped = true;      // ✅ Tạm thời disable movement (tuỳ logic)
        }
    }
}
