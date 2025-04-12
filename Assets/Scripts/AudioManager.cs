using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("AudioManager");
                instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private Dictionary<string, AudioClip> clipCache = new();

    public void PlaySFX(string clipName, Vector3 position, bool pitchRandomized = false, float volume = 1f)
    {
        AudioClip clip = GetClip(clipName);
        if (clip == null)
        {
            Debug.LogWarning($"⚠️ Không tìm thấy âm thanh: {clipName} trong Resources/Sounds/");
            return;
        }

        GameObject tempGO = new GameObject("TempSFX_" + clipName);
        tempGO.transform.position = position;

        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = 1f; // 3D
        audioSource.pitch = pitchRandomized ? Random.Range(0.85f, 1.15f) : 1f;
        audioSource.volume = volume;
        audioSource.Play();

        Destroy(tempGO, clip.length / audioSource.pitch); // Tự huỷ sau khi xong
    }

    private AudioClip GetClip(string name)
    {
        if (clipCache.TryGetValue(name, out var cachedClip))
            return cachedClip;

        var loaded = Resources.Load<AudioClip>("Sounds/" + name);
        if (loaded != null)
        {
            clipCache[name] = loaded;
            return loaded;
        }

        return null;
    }
}
