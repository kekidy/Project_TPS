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
        public WeaponType weaponType;
        public GameObject weaponObj;
    }

    [Inspector(group = "Player Status")]
    [SerializeField] private float m_maxHp = 0f;
    [SerializeField] private float m_maxShieldGauge = 0f;
    [SerializeField] private float m_maxSkillGauge  = 0f;

    [Inspector(group = "Weapon")]
    [SerializeField] private WeaponInfo[] m_weaponInfoArray     = null;
    [SerializeField] private WeaponType   m_currentWeaponType = WeaponType.NON;
    [SerializeField] private RifleCtrl    m_rifleCtrl         = null;

    [Inspector(group = "IK Target")]
    [SerializeField] private Transform leftHandTarget = null;

	private Transform m_myTransform = null;
    private Animator  m_myAnim      = null;
    private Transform m_cameraTransform  = null;
    private Vector3   m_currentDirection = Vector3.zero;
    private CapsuleCollider m_myCollider = null;
    
    private float m_ikLeftHandWeight = 1f;

    private IObservable<WeaponInfo> m_weaponInfoObservable = null;
    private WeaponInfo m_currentWeaponInfo = null;
    private WeaponInfo m_nextWeaponInfo   = null;

    public float Speed          { get { return m_myAnim.speed; } set { m_myAnim.speed = value; } }
    public float AttackMultiple { get; set; }

    public float MaxHp          { get { return m_maxHp;          } }
    public float MaxShieldGauge { get { return m_maxShieldGauge; } }
    public float MaxSkillGauge  { get { return m_maxSkillGauge;  } }

    public float CurrentHp          { get; set; }
    public float CurrentShieldGauge { get; set; }
    public float CurrentSkillGauge  { get; set; } 

	void Awake () {
        m_myTransform      = transform;
        m_myAnim           = GetComponent<Animator>();
        m_cameraTransform  = Camera.main.transform;
        m_myCollider       = GetComponent<CapsuleCollider>();
        AttackMultiple     = 1.0f;
        CurrentHp          = m_maxHp;
        CurrentShieldGauge = m_maxShieldGauge;
        CurrentSkillGauge  = m_maxSkillGauge;
        m_weaponInfoObservable = m_weaponInfoArray.ToObservable();

        m_currentWeaponInfo = m_weaponInfoArray[(int)m_currentWeaponType];
        m_currentWeaponInfo.weaponObj.SetActive(true);
        m_myAnim.SetFloat("weaponNum", (int)m_currentWeaponInfo.weaponType);
	}

    void Start()
    {
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
        if (m_currentWeaponType != WeaponType.PISTOL)
        {
            if (!m_myAnim.GetBool("isReload") && !m_myAnim.GetBool("isVault") && !m_myAnim.GetBool("isWeaponChanging"))
            {
                m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
                m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
                m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            }
            else
                m_ikLeftHandWeight = 0f;
        }
    }

    void OnEnable()
    {
        m_myAnim.SetFloat("weaponNum", (int)m_currentWeaponInfo.weaponType);
    }

    void OnDisable()
    {
        m_rifleCtrl.StopToShooting();
        WayNavigationSystem.Instance.ShowWayNavigation = false;
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Vault")
        {
            if (!m_myAnim.GetBool("isVault") && Input.GetKeyDown(KeyCode.Space))
                m_myAnim.SetTrigger("vaultTrigger");
        }
    }

    private void AttackObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_myAnim.GetBool("isReload") && !m_myAnim.GetBool("isSprint"))
            .Subscribe(_ => {
                if (Input.GetButton("Shot"))
                    StartToAttack();
                else
                    StopToAttack();
            });
    }

    private void StartToAttack()
    {
        m_myAnim.SetBool("isShot", true);
        m_rifleCtrl.StartToShooting();
    }

    private void StopToAttack()
    {
        m_myAnim.SetBool("isShot", false);
        m_rifleCtrl.StopToShooting();
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
                    m_myAnim.SetFloat("moveStartAngle", Quaternion.LookRotation(direction).eulerAngles.y);
                else
                    m_myAnim.SetFloat("moveStopAngle", m_myAnim.GetFloat("moveStartAngle"));

                m_myAnim.SetFloat("vertical", direction.z);
                m_myAnim.SetFloat("horizontal", direction.x);
                m_myAnim.SetFloat("inputMagnitude", magnitude);
            });
    }

    private void SprintObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                if (Input.GetButton("Sprint"))
                {
                    m_myAnim.SetBool("isSprint", true);
                    m_rifleCtrl.StopToShooting();
                }
                else
                    m_myAnim.SetBool("isSprint", false);
            });
    }

    private void ReloadObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_myAnim.GetBool("isReload") && (m_rifleCtrl.CurrentMagazinNum != m_rifleCtrl.MaxMagazineNum))
            .Where(_ => Input.GetButtonDown("Reload") || (m_rifleCtrl.CurrentMagazinNum == 0))
            .Subscribe(_ => {
                m_myAnim.SetBool("isReload", true);
                m_myAnim.SetBool("isSprint", false);
                m_myAnim.SetBool("isShot", false);

                m_rifleCtrl.PlayMagazineReloadSound();
                m_rifleCtrl.StopToShooting();

                TPSCameraCtrl.Instance.ZoomOn = false;
            });
    }

    private void SitObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => Input.GetButtonDown("Sit"))
            .Select(_ => !m_myAnim.GetBool("isSit"))
            .Subscribe(isSit => {
                m_myAnim.SetBool("isSit", isSit);

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
                m_nextWeaponInfo = weaponInfo;
                m_myAnim.SetBool("isWeaponChanging", true);
            });
    }

    private void CurrentWeaponActive(int isActive)
    {
        if (isActive == 1)
            m_currentWeaponInfo.weaponObj.SetActive(true);
        else
            m_currentWeaponInfo.weaponObj.SetActive(false);
    }

    public void WeaponChange()
    {
        m_currentWeaponInfo = m_nextWeaponInfo;
        m_myAnim.SetFloat("weaponNum", (int)m_currentWeaponInfo.weaponType);
    }

    public void MagazineReload()
    {
        m_rifleCtrl.MagazineReload();
    }

    private void LookAtLaterObservable()
    {
        this.LateUpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_myAnim.GetBool("isVault"))
            .Subscribe(_ => m_myTransform.eulerAngles
                = new Vector3(m_myTransform.eulerAngles.x, m_cameraTransform.eulerAngles.y, m_myTransform.eulerAngles.z));
    }

    public void AnimatorChange(string animatorName)
    {
        Resources.UnloadAsset(m_myAnim.runtimeAnimatorController);
        m_myAnim.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/" + animatorName);
    }
}
