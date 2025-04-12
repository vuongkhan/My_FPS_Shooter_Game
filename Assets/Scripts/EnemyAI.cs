using UnityEngine;
using UnityEngine.AI;

public class EnemyAI_Base : MonoBehaviour
{
    [SerializeField] protected NavMeshAgent m_NavMeshAgent; // Agent của NavMesh
    public GameObject m_Target; // Đối tượng mục tiêu (Player)
    [SerializeField] protected float speedMultiplier = 1.0f; // Tốc độ di chuyển nhân lên
    private Vector3 lastTargetPosition; // Lưu vị trí cuối cùng của mục tiêu
    private float updateThreshold = 0.5f; // Ngưỡng khoảng cách để cập nhật đường đi

    protected virtual void Start()
    {
        if (m_Target != null && m_NavMeshAgent != null)
        {
            m_NavMeshAgent.speed *= speedMultiplier; // Áp dụng tốc độ
            lastTargetPosition = m_Target.transform.position;
            m_NavMeshAgent.SetDestination(lastTargetPosition);
        }
    }

    protected virtual void Update()
    {
        if (m_NavMeshAgent == null || m_Target == null) return;

        float targetMoved = Vector3.Distance(lastTargetPosition, m_Target.transform.position);

        if (targetMoved > updateThreshold) // Kiểm tra xem mục tiêu đã di chuyển đủ xa chưa
        {
            lastTargetPosition = m_Target.transform.position;
            m_NavMeshAgent.SetDestination(lastTargetPosition);
            m_NavMeshAgent.isStopped = false; // Tiếp tục di chuyển
        }
    }
}
