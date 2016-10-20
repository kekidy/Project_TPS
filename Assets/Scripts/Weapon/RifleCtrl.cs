using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EasyEditor;

[RequireComponent(typeof(AudioSource))]
public class RifleCtrl : MonoBehaviour {
    [Inspector(group = "Status")]
    [SerializeField] private float m_damage            = 0f;
    [SerializeField] private int   m_maxMagazineNum    = 0;
    [SerializeField] private float m_rateOfFireSeconds = 0f;

    [Inspector(group = "Effect")]
    [SerializeField] private GameObject m_muzzleObj          = null;
    [SerializeField] private GameObject m_bulletImpactPrefab = null;

    [Inspector(group = "Sound")]
    [SerializeField] private GunSound m_fireSound   = null;
    [SerializeField] private GunSound m_reloadSound = null;

    [Inspector(group = "Test")]
    [SerializeField] private Text m_magazineText = null;

    public int MaxMagazineNum    { get { return m_maxMagazineNum; } }
    public int CurrentMagazinNum { get; private set; }

    private bool m_isFire = false;
    private WaitForSeconds m_fireWaitSeconds;
    private AudioSource m_myAudio;
    private Camera      m_mainCamera;
    private Transform   m_mainCmeraTransform;

	void Awake () {
        m_fireWaitSeconds = new WaitForSeconds(m_rateOfFireSeconds);
        CurrentMagazinNum = m_maxMagazineNum;
        m_myAudio         = GetComponent<AudioSource>();
        m_mainCamera         = Camera.main;
        m_mainCmeraTransform = m_mainCamera.transform;
	}

    void OnEnable()
    {
        m_magazineText.text = CurrentMagazinNum.ToString();
    }

    public void StartToShooting()
    {
        if (!m_isFire)
            StartCoroutine("Shooting");
    }

    public void StopToShooting()
    {
        m_isFire = false;
    }

    public void MagazineReload()
    {
        CurrentMagazinNum   = m_maxMagazineNum;
        m_magazineText.text = CurrentMagazinNum.ToString();
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
            m_magazineText.text = CurrentMagazinNum.ToString();
            m_muzzleObj.SetActive(false);
            m_muzzleObj.SetActive(true);

            m_fireSound.PlaySound(m_myAudio);

            if (TPSCameraCtrl.Instance.IsRayHit)
            {
                RaycastHit rayHit = TPSCameraCtrl.Instance.RayHit;
                GameObject impact = Instantiate(m_bulletImpactPrefab, rayHit.point, Quaternion.LookRotation(rayHit.normal)) as GameObject;
                Destroy(impact, 1.5f);
            }

            if (CurrentMagazinNum == 0)
            {
                m_muzzleObj.SetActive(false);
                break;
            }

            yield return m_fireWaitSeconds;

            if (m_isFire == false)
            {
                m_muzzleObj.SetActive(false);
                break;
            }
        }
    }
}
