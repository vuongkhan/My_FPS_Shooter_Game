using UnityEngine;
using System.Collections.Generic;

public enum AISkillType
{
    HeavyAttack,
    RangeAttack,
    DashAway,
    JumpBack
}

[System.Serializable]
public class AISkillData
{
    public float cooldown;
    public float maxCooldown;

    public bool IsReady => cooldown <= 0f;

    public void Tick(float deltaTime)
    {
        if (cooldown > 0f)
            cooldown -= deltaTime;
    }

    public void ResetCooldown()
    {
        cooldown = maxCooldown;
    }
}

public class AISkillManager : MonoBehaviour
{
    public Dictionary<AISkillType, AISkillData> skills = new();

    public void AddSkill(AISkillType type, float cooldown)
    {
        if (!skills.ContainsKey(type))
        {
            skills[type] = new AISkillData
            {
                cooldown = 0f,
                maxCooldown = cooldown
            };
        }
        else
        {
            Debug.LogWarning($"[AISkillManager] Skill '{type}' already exists.");
        }
    }

    public bool IsReady(AISkillType type)
    {
        return skills.TryGetValue(type, out var s) && s.IsReady;
    }

    public void UseSkill(AISkillType type)
    {
        if (skills.TryGetValue(type, out var s))
            s.ResetCooldown();
    }

    public float GetCooldown(AISkillType type)
    {
        return skills.TryGetValue(type, out var s) ? s.cooldown : -1f;
    }

    public void TickAll(float deltaTime)
    {
        foreach (var skill in skills.Values)
            skill.Tick(deltaTime);
    }

    public void DebugAll()
    {
        foreach (var kv in skills)
        {
            Debug.Log($"[Skill] {kv.Key}: {kv.Value.cooldown:F2} / {kv.Value.maxCooldown}");
        }
    }
}
