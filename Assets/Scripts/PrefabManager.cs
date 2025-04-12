using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager Instance { get; private set; }

    [System.Serializable]
    public class Entry
    {
        public string name;
        public GameObject prefab;
    }

    [Header("Prefab Registry")]
    public List<Entry> entries;

    private Dictionary<string, GameObject> prefabDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        prefabDict = new Dictionary<string, GameObject>();
        foreach (var entry in entries)
        {
            if (!string.IsNullOrEmpty(entry.name) && entry.prefab != null)
            {
                if (!prefabDict.ContainsKey(entry.name))
                    prefabDict.Add(entry.name, entry.prefab);
            }
        }
    }

    public GameObject Get(string key)
    {
        if (prefabDict.TryGetValue(key, out var prefab))
            return prefab;

        Debug.LogWarning($"⚠️ PrefabManager: Không tìm thấy key '{key}'");
        return null;
    }
}
