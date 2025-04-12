using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GameSystem : MonoBehaviour
{
    public static GameSystem Instance { get; private set; }

    public GameObject[] StartPrefabs;

    public int Score => m_Score;
    public static bool START_GAME = false;

    private int m_Score = 0;
    private int m_killedCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Instantiate startup prefabs
        foreach (var prefab in StartPrefabs)
        {
            Instantiate(prefab);
        }

        PoolSystem.Create(); // Assuming you still use PoolSystem for VFX, etc.
    }

    void Start()
    {
        // Không cần gọi WorldAudioPool.Init() nữa
        MainMenuUI.Instance.Display();
    }

    public void TargetDestroyed(int score)
    {
        m_Score += score;
        m_killedCount++;

        GameSystemInfo.Instance.UpdateScore(m_Score);

        if (SpawnManager.CanFinishGame(m_killedCount))
        {
            StartCoroutine(WinGame());
        }
    }

    public void StartGame()
    {
        START_GAME = true;
        Controller.Instance.DisplayWeapon(true);
    }

    IEnumerator WinGame()
    {
        yield return new WaitForSeconds(2);

        START_GAME = false;
        Controller.Instance.DisplayWeapon(false);
        Controller.Instance.DisplayCursor(true);

        FinalScoreUI.Instance.Display();
        GameSystemInfo.Instance.gameObject.SetActive(false);
        WeaponInfoUI.Instance.gameObject.SetActive(false);
    }
}
