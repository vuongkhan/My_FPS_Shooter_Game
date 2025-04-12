using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles visual + audio impact effects based on target material, using pooling for performance.
/// </summary>
public class ImpactManager : MonoBehaviour
{
    [System.Serializable]
    public class ImpactSetting
    {
        public ParticleSystem ParticlePrefab;
        public string ImpactSoundName; // Name of the clip in Resources/Sounds/
        public Material TargetMaterial;
    }

    public static ImpactManager Instance { get; private set; }

    [Header("Default fallback setting")]
    public ImpactSetting DefaultSettings;

    [Header("Overrides per material")]
    public List<ImpactSetting> MaterialOverrides;

    private Dictionary<Material, ImpactSetting> m_SettingLookup = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ✅ Check null để tránh crash
        if (DefaultSettings == null || DefaultSettings.ParticlePrefab == null)
        {
            Debug.LogError("❌ ImpactManager: DefaultSettings hoặc ParticlePrefab chưa được gán trong Inspector!");
            return;
        }

        // Add material overrides vào dictionary
        foreach (var setting in MaterialOverrides)
        {
            if (setting != null && setting.TargetMaterial != null && !m_SettingLookup.ContainsKey(setting.TargetMaterial))
            {
                m_SettingLookup.Add(setting.TargetMaterial, setting);
            }
        }

        // Init pool cho default particle
        PoolSystem.Instance.InitPool(DefaultSettings.ParticlePrefab, 32);

        // Init pool cho các particle khác nhau
        foreach (var setting in MaterialOverrides)
        {
            if (setting != null && setting.ParticlePrefab != null && setting.ParticlePrefab != DefaultSettings.ParticlePrefab)
            {
                PoolSystem.Instance.InitPool(setting.ParticlePrefab, 16);
            }
        }
    }

    public void PlayImpact(Vector3 position, Vector3 normal, Material material = null)
    {
        ImpactSetting setting;
        if (material == null || !m_SettingLookup.TryGetValue(material, out setting))
        {
            setting = DefaultSettings;
        }

        // Bảo vệ nếu setting vẫn null
        if (setting == null || setting.ParticlePrefab == null)
        {
            Debug.LogWarning("⚠️ ImpactManager: Không có ParticlePrefab phù hợp!");
            return;
        }

        // Get từ pool & play particle
        var ps = PoolSystem.Instance.GetInstance<ParticleSystem>(setting.ParticlePrefab);
        ps.transform.position = position;
        ps.transform.forward = normal;
        ps.gameObject.SetActive(true);
        ps.Play();

        // Gọi âm thanh nếu có
        if (!string.IsNullOrEmpty(setting.ImpactSoundName))
        {
            AudioManager.Instance.PlaySFX(setting.ImpactSoundName, position, pitchRandomized: true);
        }
    }
}
