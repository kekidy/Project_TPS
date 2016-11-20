using UnityEngine;
using System.Collections;

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
