using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class WeaponInfoUI : MonoBehaviour
{
    public static WeaponInfoUI Instance { get; private set; }

    public Text WeaponName;
    public Text WeaponClipContent;
    public Text AmmoTypeCount;
    
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void UpdateWeaponName(Weapon weapon)
    {
        WeaponName.text = weapon.name;
    }

    public void UpdateClipInfo(Weapon weapon)
    {
        WeaponClipContent.text = weapon.ClipContent.ToString();
    }

    public void UpdateAmmoAmount(int amount)
    {
        AmmoTypeCount.text = amount.ToString();
    }

    public void Display()
    {
        gameObject.SetActive(true);
    }
}
