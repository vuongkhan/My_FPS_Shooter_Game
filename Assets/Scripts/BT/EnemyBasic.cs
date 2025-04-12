using UnityEngine;

public class EnemyBasic : EnemyBase
{

    protected override void Update()
    {
        base.Update();  // Gọi Update() của EnemyBase
    }

    void Shoot()
    {
            Debug.Log("🔫 EnemyShooter bắn đạn!");
    }
}
