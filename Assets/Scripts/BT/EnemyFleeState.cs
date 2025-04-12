using UnityEngine;

public class EnemyFleeState : FSMBase
{
    public EnemyFleeState(EnemyBase enemy) : base(enemy, StatePriority.Flee) { }
    public override void Enter() => Debug.Log("Enter Flee State");
    public override void Update()
    {
        Debug.Log("Flee");
    }
    public override void Exit() => Debug.Log("Exit Flee State");
}