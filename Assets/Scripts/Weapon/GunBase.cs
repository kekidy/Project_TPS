using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyEditor;

/**
 * @brief 모든 총기류가 상속받는 클래스. 현재는 총기가 두 종류 밖에 없어서 다른 점이 없어 상속하지 않고 이 스크립트를 그대로 씀.
 */
[RequireComponent(typeof(AudioSource))]
public class GunBase : MonoBehaviour {
    [Inspector(group = "Status")]
    [SerializeField] private WeaponType m_gunType           = WeaponType.NON;
    [SerializeField] private float      m_damage            = 0f;
    [SerializeField] private int        m_maxMagazineNum    = 0;
    [SerializeField] private float      m_rateOfFireSeconds = 0f;

    [Inspector(group = "Effect")]
    [SerializeField] private GameObject m_muzzleObj          = null;
    [SerializeField] private GameObject m_bulletImpactPrefab = null;

    [Inspector(group = "Sound")]
    [SerializeField] private GunSound m_fireSound   = null;
    [SerializeField] private GunSound m_reloadSound = null;

    [SerializeField] private IceBullet iceBullet;

    public WeaponType WeaponType { get { return m_gunType; } }
    public int MaxMagazineNum    { get { return m_maxMagazineNum; } }
    public int CurrentMagazinNum { get; private set; }

    private bool m_isFire     = false;
    private bool m_isFireStop = false;

    private WaitForSeconds m_fireWaitSeconds    = null;
    private AudioSource    m_myAudio            = null;
    private Camera         m_mainCamera         = null;
    private Transform      m_mainCmeraTransform = null;

    public float AttackMutiple { get; set; }

	void Awake () {
        m_fireWaitSeconds = new WaitForSeconds(m_rateOfFireSeconds / 2f);
        CurrentMagazinNum = m_maxMagazineNum;
        m_myAudio         = GetComponent<AudioSource>();
        m_mainCamera         = Camera.main;
        m_mainCmeraTransform = m_mainCamera.transform;
        gameObject.SetActive(false);
	}

    void Update()
    {
    }

    void OnDisable()
    {
        m_isFireStop = true;
    }

    public void StartToShooting(float attackMultiple = 1f)
    {
        if (!m_isFire)
        {
            AttackMutiple = attackMultiple;
            StartCoroutine("Shooting");
        }
    }

    public void StopToShooting()
    {
        m_isFireStop = true;
    }

    public void MagazineReload()
    {
        CurrentMagazinNum = m_maxMagazineNum;
    }

    public void PlayMagazineReloadSound()
    {
        m_reloadSound.PlaySound(m_myAudio);
    }

    private IEnumerator Shooting()
    {
        m_isFire = true;

        while (true)
        {
            CurrentMagazinNum--;
            
            m_muzzleObj.SetActive(true);

            m_fireSound.PlaySound(m_myAudio);

            if (TPSCameraCtrl.Instance.IsRayHit)
            {
                RaycastHit rayHit = TPSCameraCtrl.Instance.RayHit;
                ElementalBullet elementalBullet =  ElementalBulletSystem.Instance.CurrentElementalBullet;

                var impact = Instantiate(m_bulletImpactPrefab, rayHit.point, Quaternion.LookRotation(rayHit.normal)) as GameObject;
                Destroy(impact, 1.5f);

                if (rayHit.collider.tag == "Enemy")
                {
                    var runnerBotCtrl = rayHit.collider.GetComponent<RunnerBotCtrl>();
                    runnerBotCtrl.BeAttacked(m_damage * AttackMutiple);
                    
                    
                    if (elementalBullet != null)
                    {
                        elementalBullet.OnEffect(runnerBotCtrl);
                    }
                }
            }

            yield return m_fireWaitSeconds;
            m_muzzleObj.SetActive(false);
            yield return m_fireWaitSeconds;

            if (m_isFireStop)
            {
                m_muzzleObj.SetActive(false);
                m_isFireStop = false;
                m_isFire     = false;
                break;
            }
        }
    }
}
