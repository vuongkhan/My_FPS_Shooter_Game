using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameOverUI : MonoBehaviour
{

    public static GameOverUI Instance { get; private set; }


    void Awake()
    {
        Instance = this;
       
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }
}
