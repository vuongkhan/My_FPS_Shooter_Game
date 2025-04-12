using System.Collections.Generic;
using UnityEngine;

public static class BBKeys
{
    public const string Owner = "owner";
    public const string Target = "target";
    public const string CanSeeEnemy = "canSeeEnemy";
    public const string Health = "health";
    public const string EnemyList = "enemyList";
    public const string Stamina = "stamina";
    // Thêm các key khác ở đây nếu cần
}
public class BlackboardData
{
    private Dictionary<string, object> data = new();

    public void Set<T>(string key, T value) => data[key] = value;

    public T Get<T>(string key)
    {
        if (data.TryGetValue(key, out var val) && val is T castVal)
            return castVal;

        Debug.LogWarning($"[Blackboard] Get failed: key={key}, expected type={typeof(T).Name}");
        return default;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (data.TryGetValue(key, out var val) && val is T castVal)
        {
            value = castVal;
            return true;
        }
        value = default;
        return false;
    }

    public bool HasKey(string key) => data.ContainsKey(key);
    public void Remove(string key) => data.Remove(key);
    public void Clear() => data.Clear();
    public void TrySetDefault<T>(string key, T value)
    {
        if (!HasKey(key))
            Set(key, value);
    }
    public void DebugAll()
    {
        Debug.Log("📦 BlackboardData:");
        foreach (var kv in data)
        {
            string type = kv.Value != null ? kv.Value.GetType().Name : "null";
            Debug.Log($"- {kv.Key} ({type}): {kv.Value}");
        }
    }
}
