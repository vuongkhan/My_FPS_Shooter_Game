using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{

    public static MainMenuUI Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
        Controller.Instance.DisplayCursor(true);
    }

    public void StartGame()
    {
        UIAudioPlayer.PlayPositive();
        gameObject.SetActive(false);
        Controller.Instance.DisplayCursor(false);
        GameSystemInfo.Instance.Display();
        WeaponInfoUI.Instance.Display();
        GameSystem.Instance.StartGame();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
