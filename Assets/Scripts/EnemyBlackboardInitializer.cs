using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(BlackboardBase))]
public class EnemyBlackboardInitializer : MonoBehaviour
{
    [SerializeField] protected BlackboardBase bb;

    [Header("Patrol Settings")]
    public Transform[] patrolWaypoints;
    protected float staminaRegenTimer = 0f;
    protected const float StaminaRegenInterval = 1f;
    protected const float StaminaRegenAmount = 1f;
    protected const float MaxStamina = 100f;

    protected virtual void Awake()
    {
        if (bb == null)
            bb = GetComponent<BlackboardBase>();

        if (bb == null)
        {
            Debug.LogError($"{gameObject.name} - Không tìm thấy BlackboardBase!");
            return;
        }

        Initialize(); 
    }

    public virtual void Initialize()
    {
        bb.TrySetDefault<NavMeshAgent>("agent", GetComponent<NavMeshAgent>());
        bb.TrySetDefault<Animator>("animator", GetComponent<Animator>());
        bb.TrySetDefault<GameObject>("owner", gameObject);
        bb.TrySetDefault("stamina", MaxStamina);
        bb.TrySetDefault("hp", 1000f);
        bb.TrySetDefault("speed", 1f);
        bb.TrySetDefault("lastDamage", 1f); 
        bb.TrySetDefault<string>("currentAttack", null);

        var enemyBase = GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            bb.TrySetDefault("enemy", enemyBase);
        }
        else
        {
            Debug.LogWarning($"[BBInit] {gameObject.name} không có EnemyBase.");
        }
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            bb.TrySetDefault(BBKeys.Target, player);
        }

        bb.TrySetDefault(BBKeys.CanSeeEnemy, false);

        // Gán patrol waypoints nếu có
        if (patrolWaypoints != null && patrolWaypoints.Length > 0)
        {
            bb.TrySetDefault("waypoints", patrolWaypoints);
            bb.TrySetDefault("waypointIndex", 0);
            bb.TrySetDefault("currentWaypoint", patrolWaypoints[0]);

            Debug.Log($"[BBInit] {gameObject.name} được gán {patrolWaypoints.Length} waypoint(s).");
        }
        else
        {
            Debug.LogWarning($"[BBInit] {gameObject.name} chưa gán patrolWaypoints.");
        }
    }

    protected virtual void Update()
    {
        // Cho kế thừa nếu subclass cần
    }
}
