using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSystemInfo : MonoBehaviour
{
    public static GameSystemInfo Instance { get; private set; }
    
    public Text ScoreText;
    public Text HealthText;
    public GameObject BloodImage;

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);    
    }

    public void UpdateScore(int score)
    {
        ScoreText.text = score.ToString();
    }

    public void UpdateHealth(int health)
    {
        HealthText.text = Mathf.Clamp(health, 0, 100).ToString();
    }

    public void ShowBloodEffect()
    {
        if(!BloodImage.activeSelf)
        {
            BloodImage.SetActive(true);
            StartCoroutine(HideBloodEffect());
        }        
    }

    private IEnumerator HideBloodEffect()
    {
        yield return new WaitForSeconds(1);
        BloodImage.SetActive(false);
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }

}
