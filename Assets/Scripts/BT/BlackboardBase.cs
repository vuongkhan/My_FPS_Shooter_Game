using UnityEngine;

public class BlackboardBase : MonoBehaviour
{
    public BlackboardData data = new();
    public AISkillManager skillManager;

    private void Awake()
    {
        if (skillManager == null)
        {
            skillManager = GetComponent<AISkillManager>();
            if (skillManager == null)
            {
                skillManager = gameObject.AddComponent<AISkillManager>();
                Debug.LogWarning($"[Blackboard] Auto-added AISkillManager to {gameObject.name}");
            }
        }
    }

    public void TickSkillCooldowns(float deltaTime)
    {
        skillManager?.TickAll(deltaTime);
    }
    public void TrySetDefault<T>(string key, T value)
    {
        data.TrySetDefault(key, value);
    }

    public void Set<T>(string key, T value) => data.Set(key, value);
    public T Get<T>(string key) => data.Get<T>(key);
    public bool TryGet<T>(string key, out T value) => data.TryGet(key, out value);
    public bool Has(string key) => data.HasKey(key);
    public void Remove(string key) => data.Remove(key);
}
