using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BlackboardBase))]
public class EnemyBase : MonoBehaviour
{
    public BlackboardBase blackboard { get; private set; }
    public EnemyFSMController fsmController { get; private set; }

    private void Awake()
    {
        // Gọi initializer setup BB trước
        GetComponent<EnemyBlackboardInitializer>()?.Initialize();

        // Gán BB nếu chưa có
        if (blackboard == null)
            blackboard = GetComponent<BlackboardBase>();

        if (blackboard == null)
            Debug.LogWarning($"{name} - Blackboard chưa được gán sau Initialize.");

        fsmController = new EnemyFSMController();
    }

    private void Start()
    {
        fsmController.ChangeState(new EnemyPatrolState(this));
    }

    protected virtual void Update()
    {
        UpdateVision();
        fsmController?.Update();
        SeeEnemy();
    }

    private void UpdateVision()
    {
        if (Target == null)
        {
            blackboard.Set(BBKeys.CanSeeEnemy, false);
            return;
        }

        float distance = Vector3.Distance(transform.position, Target.transform.position);
        blackboard.Set(BBKeys.CanSeeEnemy, distance <= 10f);
    }

    private void SeeEnemy()
    {
        if (Target != null && CanSeeEnemy)
        {
            fsmController.ChangeState(new EnemyChaseState(this));
        }

    }

    public GameObject Target => blackboard.Get<GameObject>(BBKeys.Target);
    public bool CanSeeEnemy => blackboard.Get<bool>(BBKeys.CanSeeEnemy);
}
