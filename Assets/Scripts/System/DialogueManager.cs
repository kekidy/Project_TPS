using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private ScriptDataBase m_scriptDataBase = null;
    [SerializeField] private GameObject     m_renderQuadObj  = null;
    
    private Text        m_text  = null;
    private AudioSource m_audio = null;

    private int m_currentMessageIndex = 0;

    public bool IsPlaying { get; private set; }

	void Awake () {
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogWarning("WayPointSystem is Exist");
        }
        else
        {
            Instance  = this;
            m_text    = GetComponent<Text>();
            m_audio   = GetComponent<AudioSource>();
            IsPlaying = false;
        }
    }
	
    public void OnDialogueMessage()
    {
        StartCoroutine("DialogueMessage");
    }

    private IEnumerator DialogueMessage()
    {
        m_renderQuadObj.SetActive(true);
        m_text.enabled = true;
        IsPlaying = true;

        do
        {
            var data = m_scriptDataBase[m_currentMessageIndex];
            string name     = data.name;
            string script   = data.script;
            float  duration = data.duration;

            if (script == string.Empty)
                m_text.text = string.Empty;
            else
                m_text.text = string.Format("<color=orange>{0}</color> : {1}", name, script);

            m_audio.clip = data.audioClip;
            m_audio.time = 0.5f;
            m_audio.Play();

            yield return new WaitForSeconds(duration);
        } while (!m_scriptDataBase[m_currentMessageIndex++].isEnd);

        m_text.enabled = false;
        m_renderQuadObj.SetActive(false);
        IsPlaying = false;
    }
}
