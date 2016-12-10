using UnityEngine;
using EasyEditor;
using System.Collections;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCtrl : MonoBehaviour {
    [System.Serializable]
    private class WeaponInfo
    {
        public KeyCode    weaponActivateKeyCode;
        public GunBase    gunBase;
    }

    [Inspector(group = "Player Status")]
    [SerializeField] private float m_maxHp = 0f;
    [SerializeField] private float m_maxShieldGauge = 0f;
    [SerializeField] private float m_maxSkillGauge  = 0f;

    [Inspector(group = "Weapon")]
    [SerializeField] private WeaponInfo[] m_weaponInfoArray  = null;
    [SerializeField] private int          m_startWeaponIndex = 0;

    [Inspector(group = "IK Target")]
    [SerializeField] private Transform leftHandTarget = null;

	private Transform m_transform = null;
    private Animator  m_anim      = null;
    private Transform m_cameraTransform  = null;
    private Vector3   m_currentDirection = Vector3.zero;
    private CapsuleCollider m_myCollider = null;
    
    private IObservable<WeaponInfo> m_weaponInfoObservable = null;
    private WeaponInfo m_currentWeaponInfo = null;
    private WeaponInfo m_nextWeaponInfo    = null;
    private GunBase    m_currentGunBase    = null;

    private float m_ikLeftHandWeight = 1f;

    public float Speed          { get { return m_anim.speed; } set { m_anim.speed = value; } }
    public float AttackMultiple { get; set; }

    public float MaxHp          { get { return m_maxHp;          } }
    public float MaxShieldGauge { get { return m_maxShieldGauge; } }
    public float MaxSkillGauge  { get { return m_maxSkillGauge;  } }

    public float CurrentHp          { get; set; }
    public float CurrentShieldGauge { get; set; }
    public float CurrentSkillGauge  { get; set; } 

    public GunBase CurrentGunBase { get { return m_currentGunBase; } }

	void Awake () {
        m_transform        = transform;
        m_anim           = GetComponent<Animator>();
        m_cameraTransform  = Camera.main.transform;
        m_myCollider       = GetComponent<CapsuleCollider>();
        AttackMultiple     = 1.0f;
        CurrentHp          = m_maxHp;
        CurrentShieldGauge = m_maxShieldGauge;
        CurrentSkillGauge  = m_maxSkillGauge;
        m_weaponInfoObservable = m_weaponInfoArray.ToObservable();

        m_currentWeaponInfo = m_weaponInfoArray[m_startWeaponIndex];
        m_currentGunBase    = m_currentWeaponInfo.gunBase;

        m_anim.SetFloat("weaponNum", (int)m_currentGunBase.WeaponType);
	}

    void Start()
    {
        m_currentGunBase.gameObject.SetActive(true);

        MoveObservable();
        AttackObservable();
        SprintObservable();
        ReloadObservable();
        SitObservable();
        WeaponChangeObservable();

        LookAtLaterObservable();
    }

    void OnAnimatorIK(int layer)
    {
        if (m_currentGunBase.WeaponType != WeaponType.PISTOL)
        {
            if (!m_anim.GetBool("isReload") && !m_anim.GetBool("isVault") && !m_anim.GetBool("isWeaponChanging"))
            {
                m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
                m_anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
                m_anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            }
            else
                m_ikLeftHandWeight = 0f;
        }
    }

    void OnEnable()
    {
        m_anim.SetFloat("weaponNum", (int)m_currentGunBase.WeaponType);
    }

    void OnDisable()
    {
        m_currentGunBase.StopToShooting();
        WayNavigationSystem.Instance.ShowWayNavigation = false;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Vault")
        {
            if (!m_anim.GetBool("isVault") && Input.GetKeyDown(KeyCode.Space))
                m_anim.SetTrigger("vaultTrigger");
        }
    }

    private void AttackObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_anim.GetBool("isReload") && !m_anim.GetBool("isSprint") && !m_anim.GetBool("isWeaponChanging"))
            .Subscribe(_ => {
                if (Input.GetButton("Shot"))
                    StartToAttack();
                else
                    StopToAttack();
            });
    }

    private void StartToAttack()
    {
        m_anim.SetBool("isShot", true);
        m_currentGunBase.StartToShooting();
    }

    private void StopToAttack()
    {
        m_anim.SetBool("isShot", false);
        m_currentGunBase.StopToShooting();
    }

    private void MoveObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Select(_ => new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")))
            .DistinctUntilChanged()
            .Subscribe(direction => {
                float magnitude = direction.magnitude;
                if (magnitude >= 0.5f)
                    m_anim.SetFloat("moveStartAngle", Quaternion.LookRotation(direction).eulerAngles.y);
                else
                    m_anim.SetFloat("moveStopAngle", m_anim.GetFloat("moveStartAngle"));

                m_anim.SetFloat("vertical", direction.z);
                m_anim.SetFloat("horizontal", direction.x);
                m_anim.SetFloat("inputMagnitude", magnitude);
            });
    }

    private void SprintObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                if (Input.GetButton("Sprint"))
                {
                    m_anim.SetBool("isSprint", true);
                    m_currentGunBase.StopToShooting();
                }
                else
                    m_anim.SetBool("isSprint", false);
            });
    }

    private void ReloadObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_anim.GetBool("isReload") && (m_currentGunBase.CurrentMagazinNum != m_currentGunBase.MaxMagazineNum))
            .Where(_ => Input.GetButtonDown("Reload") || (m_currentGunBase.CurrentMagazinNum <= 0))
            .Subscribe(_ => {
                m_anim.SetBool("isReload", true);
                m_anim.SetBool("isSprint", false);
                m_anim.SetBool("isShot", false);

                m_currentGunBase.PlayMagazineReloadSound();
                m_currentGunBase.StopToShooting();

                TPSCameraCtrl.Instance.ZoomOn = false;
            });
    }

    private void SitObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => Input.GetButtonDown("Sit"))
            .Select(_ => !m_anim.GetBool("isSit"))
            .Subscribe(isSit => {
                m_anim.SetBool("isSit", isSit);

                if (isSit)
                {
                    m_myCollider.center = new Vector3(0f, 0.4f, 0f);
                    m_myCollider.height = 0.8f;
                }
                else
                {
                    m_myCollider.center = new Vector3(0f, 0.8f, 0f);
                    m_myCollider.height = 1.6f;
                }
            });
    }

    private void WeaponChangeObservable()
    {
        this.UpdateAsObservable()
            .SelectMany(_ => m_weaponInfoObservable.Where(weaponInfo => Input.GetKeyDown(weaponInfo.weaponActivateKeyCode)))
            .Where(weapon => weapon != m_currentWeaponInfo)
            .Subscribe(weaponInfo => {
                m_currentGunBase.StopToShooting();
                m_nextWeaponInfo = weaponInfo;
                m_anim.SetBool("isWeaponChanging", true);
            });
    }

    private void CurrentWeaponActive(int isActive)
    {
        if (isActive == 1)
            m_currentGunBase.gameObject.SetActive(true);
        else
            m_currentGunBase.gameObject.SetActive(false);
    }

    public void WeaponChange()
    {
        m_currentWeaponInfo = m_nextWeaponInfo;
        m_currentGunBase    = m_currentWeaponInfo.gunBase;
        m_anim.SetFloat("weaponNum", (int)m_currentGunBase.WeaponType);
    }

    public void MagazineReload()
    {
        m_currentGunBase.MagazineReload();
    }

    private void LookAtLaterObservable()
    {
        this.LateUpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_anim.GetBool("isVault"))
            .Subscribe(_ => m_transform.eulerAngles
                = new Vector3(m_transform.eulerAngles.x, m_cameraTransform.eulerAngles.y, m_transform.eulerAngles.z));
    }

    public void AnimatorChange(string animatorName)
    {
        Resources.UnloadAsset(m_anim.runtimeAnimatorController);
        m_anim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/" + animatorName);
    }

    public void BeAttacked(float damage)
    {
        CurrentHp -= damage;
    }
}
