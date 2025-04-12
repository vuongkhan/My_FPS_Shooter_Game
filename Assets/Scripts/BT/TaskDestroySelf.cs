using UnityEngine;

public class TaskDestroySelf : Node
{
    private float destroyDelay = 2f;
    private bool started = false;
    private float timer = 0f;

    public override NodeState Evaluate(BlackboardBase blackboard)
    {
        if (!blackboard.TryGet<EnemyBase>("enemy", out var enemy))
        {
            return NodeState.FAILURE;
        }

        if (!started)
        {
            timer = 0f;
            started = true;
            return NodeState.RUNNING;
        }

        timer += Time.deltaTime;
        if (timer >= destroyDelay)
        {
            GameObject.Destroy(enemy.gameObject);
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }
}
