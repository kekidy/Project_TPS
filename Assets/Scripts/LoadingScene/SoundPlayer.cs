using UnityEngine;
using System.Collections;

/**
 * @brief Main Scene의 사운드 플레이를 위한 컨트롤러 스크립트
 */

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour {
    [SerializeField] private AudioClip[] m_audioClipArray = null;

    private AudioSource m_audio = null;

	void Start () {
        m_audio = GetComponent<AudioSource>();
        StartCoroutine("SoundPlay");
	}

    private IEnumerator SoundPlay()
    {
        for (int i = 0; i < m_audioClipArray.Length; i++)
        {
            m_audio.clip = m_audioClipArray[i];
            m_audio.Play();

            while (m_audio.isPlaying)
                yield return null;
        }
    }
}
