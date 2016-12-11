using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/**
 * @brief 게임 중간 중간에 나오는 각종 대사들을 제어하기 위한 컨트롤러 스크립트
 */
[RequireComponent(typeof(Text))]
[RequireComponent(typeof(AudioSource))]
public class DialogueManager : MonoBehaviour {
    [System.Serializable]
    private class DialogueRenderInfo
    {
        public string    targetName;
        public Transform targetPosTansform;
    }

    public static DialogueManager Instance { get; private set; }
    
    [SerializeField] private Transform      m_renderCamera   = null;
    [SerializeField] private ScriptDataBase m_scriptDataBase = null;
    [SerializeField] private GameObject     m_renderQuadObj  = null;
    [SerializeField] private DialogueRenderInfo[] m_dialogueRenderInfo = null;

    private Dictionary<string, Vector3> m_cameraPosDic = new Dictionary<string, Vector3>();

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

            for (int i = 0; i < m_dialogueRenderInfo.Length; i++)
                m_cameraPosDic.Add(m_dialogueRenderInfo[i].targetName, m_dialogueRenderInfo[i].targetPosTansform.position);
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

            if (name != string.Empty)
                m_renderCamera.position = m_cameraPosDic[name];

            yield return new WaitForSeconds(duration);
        } while (!m_scriptDataBase[m_currentMessageIndex++].isEnd);

        m_text.enabled = false;
        m_renderQuadObj.SetActive(false);
        IsPlaying = false;
    }
}
