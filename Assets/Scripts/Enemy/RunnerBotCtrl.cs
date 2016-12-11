using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using BehaviorTree;
using EasyEditor;
using UniLinq;

/**
 * @brief 적 유닛인 RunnerBot의 컨트롤러 스크립트
 */

[RequireComponent(typeof(AudioSource))]
public class RunnerBotCtrl : MonoBehaviour {
    [Inspector(group = "Runner Bot Status")]
    [SerializeField] private float m_hp                = 0f;
    [SerializeField] private float m_damage            = 0f;

    [Inspector(group = "Weapon Info")]
    [SerializeField] private float m_attackDelay       = 0f;
    [SerializeField] private float m_oneCycleDelay     = 0f;
    [SerializeField] private float m_oneCycleAttackNum = 0f;  
    [SerializeField] private GameObject m_muzzleFlashObj    = null;
    [SerializeField] private GameObject m_bulletImpactObj   = null;
    [SerializeField] private Transform  m_weaponBulletPoint = null;
    [SerializeField] private GunSound   m_fireSound         = null;

    [Inspector(group = "IK Info")]
    [SerializeField] private Transform m_leftHandTarge = null;

    private Transform   m_transform       = null;
    private Animator    m_myAnim          = null;   
    private Transform   m_targetTransform = null;
    private AudioSource m_audio           = null;
    private PlayerCtrl  m_playerCtrl      = null;

    private WaitForSeconds m_waitForAttackDelay = null;
    private WaitForSeconds m_waitForOneCycle    = null;

    private float   m_ikLeftHandWeight  = 1f; 
    private float[] m_elementalAccrue   = new float[2];
    private bool    m_isPlayerDetact    = false;

    public bool IsPlayerDetect
    {
        get { return m_isPlayerDetact; }
        set
        {
            m_isPlayerDetact = value;
            m_myAnim.SetBool("isPlayerDetect", value);
        }
    }

    public bool IsHit              { get { return m_myAnim.GetBool("isHit"); } }
    public bool IsDead             { get { return m_hp <= 0f; } }
    public bool IsAttacking        { get; set; }
    public bool IsOnCondition      { get; set; }
    public float[] ElementalAccrue { get { return m_elementalAccrue; } }

    public Animator Anim { get { return m_myAnim; } }

	void Awake () {
        m_transform          = transform;
        m_myAnim             = GetComponent<Animator>();
        m_audio              = GetComponent<AudioSource>();
        m_playerCtrl         = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
        m_targetTransform    = m_playerCtrl.HeadTransform;
        m_waitForAttackDelay = new WaitForSeconds(m_attackDelay / 2);
        m_waitForOneCycle    = new WaitForSeconds(m_oneCycleDelay);
        IsAttacking          = false;

        AngleUpdateObservable();
        TargetSensingUpdate();
    }

    void OnAnimatorIK(int layer)
    {
        m_ikLeftHandWeight = Mathf.MoveTowards(m_ikLeftHandWeight, 1f, Time.smoothDeltaTime * 2f);
        m_myAnim.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_ikLeftHandWeight);
        m_myAnim.SetIKPosition(AvatarIKGoal.LeftHand, m_leftHandTarge.position);
    }


    private void AngleUpdateObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => m_isPlayerDetact)
            .Subscribe(_ => {
                Vector3    dir     = (m_targetTransform.position - m_transform.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir);
                float      rotY    = lookRot.eulerAngles.y - m_transform.rotation.eulerAngles.y;
                float      rotX    = lookRot.eulerAngles.x < 180 ? lookRot.eulerAngles.x : lookRot.eulerAngles.x - 360f; 
                
                m_myAnim.SetFloat("horAngle", rotY);
                m_myAnim.SetFloat("verAngle", -rotX);
            });
    }

    private void TargetSensingUpdate()
    {
        this.UpdateAsObservable()
            .Where(_ => !m_isPlayerDetact)
            .Subscribe(_ => {
                Vector3    dir     = (m_targetTransform.position - m_weaponBulletPoint.position).normalized;
                Quaternion lookRot = Quaternion.LookRotation(dir);
                float      rotY    = lookRot.eulerAngles.y - m_transform.rotation.eulerAngles.y;

                RaycastHit hit;
                if (Mathf.Abs(rotY) <= 90f)
                {
                    if (Physics.Raycast(m_weaponBulletPoint.position, dir, out hit, Mathf.Infinity))
                    {
                        if (hit.transform.tag == "Player")
                            IsPlayerDetect = true;
                    }
                }
            });
    }

    public void BeAttacked(float damage)
    {
        m_hp -= damage;

        if (Random.Range(0f, 1f) < 0.4f)
            m_myAnim.SetBool("isHit", true);

        if (!IsPlayerDetect)
        {
            IsPlayerDetect = true;

            var runnerBotArray = FindObjectsOfType<RunnerBotCtrl>()
                .Where(runnerBot => Vector3.Distance(m_transform.position, runnerBot.m_transform.position) < 3f)
                .ToArray();

            for (int i = 0; i < runnerBotArray.Length; i++)
                runnerBotArray[i].IsPlayerDetect = true;
        }
    }

    public void StartAttackToTarget()
    {
        if (!IsAttacking)
            StartCoroutine("AttackToTarget");
    }

    public void IncreaseElementalAccrue(float value, ElementalType type)
    {
        m_elementalAccrue[(int)type] += value;
    }

    public void ResetElementalAccure()
    {
        for (int i = 0; i < m_elementalAccrue.Length; i++)
            m_elementalAccrue[i] = 0f;
    }

    private IEnumerator AttackToTarget()
    {
        IsAttacking = true;

        for (int i = 0; i < m_oneCycleAttackNum; i++)
        {
            yield return m_waitForAttackDelay;

            m_muzzleFlashObj.SetActive(true);
            Vector3 dir = (m_targetTransform.position - m_weaponBulletPoint.position).normalized;
            Vector3 finalAttackDir = dir + new Vector3(dir.z * Random.Range(-0.1f, 0.1f), 0f, dir.x * Random.Range(-0.1f, 0.1f));

            m_fireSound.PlaySound(m_audio);

            RaycastHit hit;
            if (Physics.Raycast(m_weaponBulletPoint.position, finalAttackDir, out hit, Mathf.Infinity))
            {
                if (hit.transform.tag == "Player")
                {
                    var impact = Instantiate(m_bulletImpactObj, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject;
                    Destroy(impact, 1.5f);

                    m_playerCtrl.BeAttacked(m_damage);
                }
            }

            yield return m_waitForAttackDelay;
            m_muzzleFlashObj.SetActive(false);
        }

        yield return m_waitForOneCycle;
        IsAttacking = false;
    }
}
