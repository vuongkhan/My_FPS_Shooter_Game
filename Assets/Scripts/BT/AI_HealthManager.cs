using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_HealthManager : MonoBehaviour
{
    [SerializeField] protected BlackboardBase bb;
    public EnemyFSMController fsmController { get; private set; }

    protected float staminaRegenTimer = 0f;
    protected const float StaminaRegenInterval = 1f;
    protected const float StaminaRegenAmount = 1f;
    protected const float MaxStamina = 100f;
    private EnemyBase enemyBase;
    protected virtual void Awake()
    {
        if (bb == null)
         bb = GetComponent<BlackboardBase>();
        enemyBase = GetComponent<EnemyBase>();

        if (enemyBase != null)
        {
            fsmController = enemyBase.fsmController; 
        }
        else
        {
            Debug.LogError("❌ Không tìm thấy EnemyBase!");
        }
    }


    void Update()
    {
        RegenerateStamina();
        fsmController?.Update();
    }

    protected virtual void RegenerateStamina()
    {
        staminaRegenTimer += Time.deltaTime;
        if (staminaRegenTimer >= StaminaRegenInterval)
        {
            staminaRegenTimer = 0f;

            if (bb.TryGet<float>("stamina", out float currentStamina))
            {
                if (currentStamina < MaxStamina)
                {
                    float newStamina = Mathf.Min(currentStamina + StaminaRegenAmount, MaxStamina);
                    bb.Set("stamina", newStamina);
                    Debug.Log($"💚 Hồi stamina: {currentStamina} ➡️ {newStamina}");
                }
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        Debug.Log($"💥 Nhận sát thương: {damage}");
        bb.Set("lastDamage", damage);
        if (bb.TryGet<float>("hp", out float currentHP))
        {
            float newHP = Mathf.Max(currentHP - damage, 0f); 
            bb.Set("hp", newHP);
            Debug.Log($"❤️ HP: {currentHP} ➖ {damage} = {newHP}");
            if (damage >= 50f)
            {
                fsmController.ChangeState(new EnemyStunState(enemyBase));
                Debug.Log("😵 Vào trạng thái bị khống chế!");
            }
            if (newHP <= 0f)
            {
                fsmController.ChangeState(new EnemyDieState(enemyBase));
            }
        }
        else
        {
            Debug.LogWarning("❌ Không tìm thấy 'hp' trong Blackboard.");
        }
    }


}
