using UnityEngine;
using System.Collections;

/**
 * @brief 문의 잠금, 열리는 애니메이션의 제어를 담당하는 컨트롤러 스크립트
 */

public class Animation_door : MonoBehaviour {
    [SerializeField] private bool       m_isAvaliableDoor = true;
    [SerializeField] private GameObject m_closeHoloObj    = null;

    [Header("Sound")]
	[SerializeField] private AudioClip m_doorOpenClip  = null;
	[SerializeField] private AudioClip m_doorCloseClip = null;
    [SerializeField] private AudioClip m_bebClip       = null;

    private Animator    m_myAnim     = null;
    private BoxCollider m_myCollider = null;
    private AudioSource m_audio      = null;

    public bool IsAvaliable
    {
        get { return m_isAvaliableDoor; }
        set
        {
            m_isAvaliableDoor    = value;
            m_myCollider.enabled = m_isAvaliableDoor;

            if (m_bebClip)
                m_audio.PlayOneShot(m_bebClip, 1f);

            if (m_closeHoloObj != null)
                m_closeHoloObj.SetActive(!m_isAvaliableDoor);

        }
    }

	void Awake()
	{
        m_myAnim = GetComponent<Animator>();
        m_myCollider = GetComponent<BoxCollider>();
        m_audio = GetComponent<AudioSource>();

        if (!m_isAvaliableDoor)
            m_myCollider.enabled = m_isAvaliableDoor;

        if (m_closeHoloObj != null)
            m_closeHoloObj.SetActive(!m_isAvaliableDoor);

    }

	void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            m_myAnim.SetBool("Open", true);
            m_audio.clip = m_doorOpenClip;
            m_audio.Play();
        }
    }

	void OnTriggerExit(Collider other) {
        if (other.GetComponent<Collider>().tag == "Player")
        {
            m_myAnim.SetBool("Open", false);
            m_audio.clip = m_doorCloseClip;
            m_audio.Play();
        }
    }
}
