using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }
    public int health = 100;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyDamage(int damage)
    {
        health -= damage;
        GameSystemInfo.Instance.UpdateHealth(health);
        GameSystemInfo.Instance.ShowBloodEffect();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, 100);
        GameSystemInfo.Instance.UpdateHealth(health);
    }

    private void Die()
    {
        GameSystem.START_GAME = false;
        GameOverUI.Instance.Display();
        Controller.Instance.DisplayCursor(true);
        Controller.Instance.DisplayWeapon(false);
        WeaponInfoUI.Instance.gameObject.SetActive(false);
    }
}
