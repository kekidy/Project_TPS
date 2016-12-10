using UnityEngine;
using System.Collections;

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
