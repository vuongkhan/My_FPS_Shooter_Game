using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Weapon : MonoBehaviour
{
    static RaycastHit[] s_HitInfoBuffer = new RaycastHit[8];
    
    public enum TriggerType
    {
        Auto,
        Manual
    }

    public enum WeaponType
    {
        Raycast,
        Projectile
    }

    public enum WeaponState
    {
        Idle,
        Firing,
        Reloading
    }

    [System.Serializable]
    public class AdvancedSettings
    {
        public float spreadAngle = 0.0f;
        public int projectilePerShot = 1;
        public float screenShakeMultiplier = 1.0f;
    }

    public TriggerType triggerType = TriggerType.Manual;
    public WeaponType weaponType = WeaponType.Raycast;
    public float fireRate = 0.5f;
    public float reloadTime = 2.0f;
    public int clipSize = 4;
    public float damage = 1.0f;

    [AmmoType]
    public int ammoType = -1;

    public Projectile projectilePrefab;
    public float projectileLaunchForce = 200.0f;

    public Transform EndPoint; 

    public AdvancedSettings advancedSettings;
    
    [Header("Animation Clips")]
    public AnimationClip FireAnimationClip;
    public AnimationClip ReloadAnimationClip;

    [Header("Audio Clips")]
    public string FireSFXName = "weapon_fire";
    public string ReloadSFXName = "weapon_reload";

    [Header("Visual Settings")]
    public LineRenderer PrefabRayTrail;
    

    public bool triggerDown
    {
        get { return m_TriggerDown; }
        set 
        { 
            m_TriggerDown = value;
            if (!m_TriggerDown) m_ShotDone = false;
        }
    }

    public WeaponState CurrentState => m_CurrentState;
    public int ClipContent => m_ClipContent;
    public Controller Owner => m_Owner;

    Controller m_Owner;
    
    Animator m_Animator;
    WeaponState m_CurrentState;
    bool m_ShotDone;
    float m_ShotTimer = -1.0f;
    bool m_TriggerDown;
    int m_ClipContent;

    AudioSource m_Source;

    Vector3 m_ConvertedMuzzlePos;

    class ActiveTrail
    {
        public LineRenderer renderer;
        public Vector3 direction;
        public float remainingTime;
    }
    
    List<ActiveTrail> m_ActiveTrails = new List<ActiveTrail>();
    
    Queue<Projectile> m_ProjectilePool = new Queue<Projectile>();
    
    int fireNameHash = Animator.StringToHash("fire");
    int reloadNameHash = Animator.StringToHash("reload");     

    void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_ClipContent = clipSize;

        if (PrefabRayTrail != null)
        {
            const int trailPoolSize = 16;
            PoolSystem.Instance.InitPool(PrefabRayTrail, trailPoolSize);
        }

        if (projectilePrefab != null)
        {
            int size = Mathf.Max(4, clipSize) * advancedSettings.projectilePerShot;
            for (int i = 0; i < size; ++i)
            {
                Projectile p = Instantiate(projectilePrefab);
                p.gameObject.SetActive(false);
                m_ProjectilePool.Enqueue(p);
            }
        }
    }

    public void PickedUp(Controller c)
    {
        m_Owner = c;
    }

    public void PutAway()
    {
        m_Animator.WriteDefaultValues();
        
        for (int i = 0; i < m_ActiveTrails.Count; ++i)
        {
            var activeTrail = m_ActiveTrails[i];
            m_ActiveTrails[i].renderer.gameObject.SetActive(false);
        }
        
        m_ActiveTrails.Clear();
    }

    public void Selected()
    {
        var ammoRemaining = m_Owner.GetAmmo(ammoType);
        
        
        if(FireAnimationClip != null)
            m_Animator.SetFloat("fireSpeed",  FireAnimationClip.length / fireRate);
        
        if(ReloadAnimationClip != null)
            m_Animator.SetFloat("reloadSpeed", ReloadAnimationClip.length / reloadTime);
        
        m_CurrentState = WeaponState.Idle;

        triggerDown = false;
        m_ShotDone = false;
        
        WeaponInfoUI.Instance.UpdateWeaponName(this);
        WeaponInfoUI.Instance.UpdateClipInfo(this);
        WeaponInfoUI.Instance.UpdateAmmoAmount(m_Owner.GetAmmo(ammoType));
        


        if (m_ClipContent == 0 && ammoRemaining != 0)
        { 
       
            int chargeInClip = Mathf.Min(ammoRemaining, clipSize);
            m_ClipContent += chargeInClip;        
       
            m_Owner.ChangeAmmo(ammoType, -chargeInClip);       
            WeaponInfoUI.Instance.UpdateClipInfo(this);
        }
        
        m_Animator.SetTrigger("selected");
    }

    public void Fire()
    {
        if (m_CurrentState != WeaponState.Idle || m_ShotTimer > 0 || m_ClipContent == 0)
            return;
        
        m_ClipContent -= 1;
        
        m_ShotTimer = fireRate;

        
        WeaponInfoUI.Instance.UpdateClipInfo(this);

        //the state will only change next frame, so we set it right now.
        m_CurrentState = WeaponState.Firing;
        
        m_Animator.SetTrigger("fire");
        AudioManager.Instance.PlaySFX(FireSFXName, transform.position, pitchRandomized: true);

        CameraShaker.Instance.Shake(0.2f, 0.05f * advancedSettings.screenShakeMultiplier);

        if (weaponType == WeaponType.Raycast)
        {
            for (int i = 0; i < advancedSettings.projectilePerShot; ++i)
            {
                RaycastShot();
            }
        }
        else
        {
            ProjectileShot();
        }
    }


    void RaycastShot()
    {
        float spreadRatio = advancedSettings.spreadAngle / Controller.Instance.MainCamera.fieldOfView;
        Vector2 spread = spreadRatio * Random.insideUnitCircle;

        Ray r = Controller.Instance.MainCamera.ViewportPointToRay(Vector3.one * 0.5f + (Vector3)spread);
        RaycastHit hit;

        Vector3 hitPosition = r.origin + r.direction * 200.0f;

        if (Physics.Raycast(r, out hit, 1000.0f, ~(1 << 9), QueryTriggerInteraction.Ignore))
        {
            Renderer renderer = hit.collider.GetComponentInChildren<Renderer>();
            ImpactManager.Instance.PlayImpact(hit.point, hit.normal, renderer == null ? null : renderer.sharedMaterial);

            if (hit.distance > 5.0f)
                hitPosition = hit.point;

            // ✅ Gọi AI_HealthManager nếu object có tag là Enemy
            if (hit.collider.CompareTag("Enemy"))
            {
                AI_HealthManager health = hit.collider.GetComponent<AI_HealthManager>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }

        if (PrefabRayTrail != null)
        {
            var pos = new Vector3[] { GetCorrectedMuzzlePlace(), hitPosition };
            var trail = PoolSystem.Instance.GetInstance<LineRenderer>(PrefabRayTrail);
            trail.gameObject.SetActive(true);
            trail.SetPositions(pos);
            m_ActiveTrails.Add(new ActiveTrail()
            {
                remainingTime = 0.3f,
                direction = (pos[1] - pos[0]).normalized,
                renderer = trail
            });
        }
    }


    void ProjectileShot()
    {
        for (int i = 0; i < advancedSettings.projectilePerShot; ++i)
        {
            float angle = Random.Range(0.0f, advancedSettings.spreadAngle * 0.5f);
            Vector2 angleDir = Random.insideUnitCircle * Mathf.Tan(angle * Mathf.Deg2Rad);

            Vector3 dir = EndPoint.transform.forward + (Vector3)angleDir;
            dir.Normalize();

            var p = m_ProjectilePool.Dequeue();
            
            p.gameObject.SetActive(true);
            p.Launch(this, dir, projectileLaunchForce);
        }
    }

    public void ReturnProjecticle(Projectile p)
    {
        m_ProjectilePool.Enqueue(p);
    }

    public void Reload()
    {
        if (m_CurrentState != WeaponState.Idle || m_ClipContent == clipSize)
            return;

        int remainingBullet = m_Owner.GetAmmo(ammoType);

        if (remainingBullet == 0)
        {
            return;
        }


        if (ReloadSFXName != null)
        {
            AudioManager.Instance.PlaySFX(ReloadSFXName, transform.position, pitchRandomized: true);
        }

        int chargeInClip = Mathf.Min(remainingBullet, clipSize - m_ClipContent);
     
        m_CurrentState = WeaponState.Reloading;
        
        m_ClipContent += chargeInClip;
        
        m_Animator.SetTrigger("reload");
        
        m_Owner.ChangeAmmo(ammoType, -chargeInClip);
        
        WeaponInfoUI.Instance.UpdateClipInfo(this);
    }

    void Update()
    {
        UpdateControllerState();        
        
        if (m_ShotTimer > 0)
            m_ShotTimer -= Time.deltaTime;

        Vector3[] pos = new Vector3[2];
        for (int i = 0; i < m_ActiveTrails.Count; ++i)
        {
            var activeTrail = m_ActiveTrails[i];
            
            activeTrail.renderer.GetPositions(pos);
            activeTrail.remainingTime -= Time.deltaTime;

            pos[0] += activeTrail.direction * 50.0f * Time.deltaTime;
            pos[1] += activeTrail.direction * 50.0f * Time.deltaTime;
            
            m_ActiveTrails[i].renderer.SetPositions(pos);
            
            if (m_ActiveTrails[i].remainingTime <= 0.0f)
            {
                m_ActiveTrails[i].renderer.gameObject.SetActive(false);
                m_ActiveTrails.RemoveAt(i);
                i--;
            }
        }
    }

    void UpdateControllerState()
    {
        
        var info = m_Animator.GetCurrentAnimatorStateInfo(0);

        WeaponState newState;
        if (info.shortNameHash == fireNameHash)
            newState = WeaponState.Firing;
        else if (info.shortNameHash == reloadNameHash)
            newState = WeaponState.Reloading;
        else
            newState = WeaponState.Idle;

        if (newState != m_CurrentState)
        {
            var oldState = m_CurrentState;
            m_CurrentState = newState;
            
            if (oldState == WeaponState.Firing)
            {//we just finished firing, so check if we need to auto reload
                if(m_ClipContent == 0)
                    Reload();
            }
        }

        if (triggerDown)
        {
            if (triggerType == TriggerType.Manual)
            {
                if (!m_ShotDone)
                {
                    m_ShotDone = true;
                    Fire();
                }
            }
            else
                Fire();
        }
    }
    
    public Vector3 GetCorrectedMuzzlePlace()
    {
        Vector3 position = EndPoint.position;

        position = Controller.Instance.WeaponCamera.WorldToScreenPoint(position);
        position = Controller.Instance.MainCamera.ScreenToWorldPoint(position);

        return position;
    }
    void Aim()
    {

    }
}

public class AmmoTypeAttribute : PropertyAttribute
{
    
}

public abstract class AmmoDisplay : MonoBehaviour
{
    public abstract void UpdateAmount(int current, int max);
}

