using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance { get; private set; }

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

    public void OpenEpisode()
    {
        UIAudioPlayer.PlayPositive();
        gameObject.SetActive(false);
    }

    public void ReturnToGame()
    {
        UIAudioPlayer.PlayPositive();
        gameObject.SetActive(false);
        Controller.Instance.DisplayCursor(false);
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
