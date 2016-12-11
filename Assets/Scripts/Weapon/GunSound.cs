using UnityEngine;
using System.Collections;

/**
 * @brief 총기 사운드의 다양한 정보를 가지고 있는 클래스. 보유한 정보를 기반으로 사운드를 출력함
 */
[System.Serializable]
public class GunSound
{
    [SerializeField] private AudioClip m_audioClip;
    [SerializeField] private float     m_volume;
    [SerializeField] private float     m_pitch;
    [SerializeField] private float     m_time;
    [SerializeField] private bool      m_isPlayOneShot;

    public void PlaySound(AudioSource audioSource)
    {
        audioSource.pitch  = m_pitch;

        if (m_isPlayOneShot)
            audioSource.PlayOneShot(m_audioClip, m_volume);
        else
        {
            audioSource.time   = m_time;
            audioSource.volume = m_volume;
            audioSource.clip   = m_audioClip;
            audioSource.Play();
        }
    }
}
