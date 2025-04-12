using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmmoInventoryEntry
{
    [AmmoType]
    public int ammoType;
    public int amount = 0;
}

public class Controller : MonoBehaviour
{
    public static Controller Instance { get; private set; }

    public Camera MainCamera;
    public Camera WeaponCamera;
    public Transform WeaponPosition;

    public Weapon[] startingWeapons;
    public AmmoInventoryEntry[] startingAmmo;

    private bool isPaused = false;
    private int currentWeapon;
    private List<Weapon> weapons = new List<Weapon>();
    private Dictionary<int, int> ammoInventory = new Dictionary<int, int>();

    public bool LockControl { get; set; }
    public bool CanPause { get; set; } = true;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isPaused = false;

        for (int i = 0; i < startingWeapons.Length; i++)
        {
            PickupWeapon(startingWeapons[i]);
        }

        for (int i = 0; i < startingAmmo.Length; i++)
        {
            ChangeAmmo(startingAmmo[i].ammoType, startingAmmo[i].amount);
        }

        currentWeapon = -1;
        ChangeWeapon(0);
    }

    private void Update()
    {
        if (!GameSystem.START_GAME)
            return;

        if (CanPause && Input.GetButtonDown("Menu"))
            PauseMenu.Instance.Display();

        if (!isPaused && !LockControl)
        {
            HandleWeaponInput();
        }
    }

    void HandleWeaponInput()
    {
        weapons[currentWeapon].triggerDown = Input.GetMouseButton(0);

        if (Input.GetButton("Reload"))
            weapons[currentWeapon].Reload();

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
            ChangeWeapon(currentWeapon - 1);
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            ChangeWeapon(currentWeapon + 1);

        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                int num = (i == 0) ? 10 : i - 1;
                if (num < weapons.Count)
                    ChangeWeapon(num);
            }
        }
    }

    public void DisplayCursor(bool display)
    {
        isPaused = display;
        Cursor.lockState = display ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = display;
    }

    void PickupWeapon(Weapon prefab)
    {
        if (weapons.Exists(weapon => weapon.name == prefab.name))
        {
            ChangeAmmo(prefab.ammoType, prefab.clipSize);
        }
        else
        {
            var w = Instantiate(prefab, WeaponPosition, false);
            w.name = prefab.name;
            w.transform.localPosition = Vector3.zero;
            w.transform.localRotation = Quaternion.identity;
            w.gameObject.SetActive(false);
            w.PickedUp(this);
            weapons.Add(w);
        }
    }

    void ChangeWeapon(int number)
    {
        if (currentWeapon != -1)
        {
            weapons[currentWeapon].PutAway();
            weapons[currentWeapon].gameObject.SetActive(false);
        }

        currentWeapon = number;
        if (currentWeapon < 0) currentWeapon = weapons.Count - 1;
        else if (currentWeapon >= weapons.Count) currentWeapon = 0;

        weapons[currentWeapon].gameObject.SetActive(true);
        weapons[currentWeapon].Selected();
    }

    public int GetAmmo(int ammoType)
    {
        return ammoInventory.TryGetValue(ammoType, out int value) ? value : 0;
    }

    public void ChangeAmmo(int ammoType, int amount)
    {
        if (!ammoInventory.ContainsKey(ammoType))
            ammoInventory[ammoType] = 0;

        var previous = ammoInventory[ammoType];
        ammoInventory[ammoType] = Mathf.Clamp(ammoInventory[ammoType] + amount, 0, 999);

        if (weapons[currentWeapon].ammoType == ammoType)
        {
            if (previous == 0 && amount > 0)
            {
                weapons[currentWeapon].Selected();
            }
            WeaponInfoUI.Instance.UpdateAmmoAmount(GetAmmo(ammoType));
        }
    }

    public void DisplayWeapon(bool display)
    {
        WeaponPosition.gameObject.SetActive(display);
    }
}
