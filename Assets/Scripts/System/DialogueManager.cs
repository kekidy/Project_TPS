using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Text))]
public class DialogueManager : MonoBehaviour {
    public static DialogueManager Instance { get; private set; }

    [SerializeField] private ScriptDataBase m_scriptDataBase;
    [SerializeField] private GameObject     m_renderQuadObj;
    
    private Text m_text = null;
    private int m_currentMessageIndex = 0;

	void Awake () {
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogWarning("WayPointSystem is Exist");
        }
        else
        {
            Instance = this;
            m_text   = GetComponent<Text>();
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

        do
        {
            string name     = m_scriptDataBase[m_currentMessageIndex].name;
            string script   = m_scriptDataBase[m_currentMessageIndex].script;
            float  duration = m_scriptDataBase[m_currentMessageIndex].duration;
            m_text.text = string.Format("<color=yellow>{0}</color> : {1}", name, script);
            yield return new WaitForSeconds(duration);
        } while (!m_scriptDataBase[m_currentMessageIndex++].isEnd);

        m_text.enabled = false;
        m_renderQuadObj.SetActive(false);
    }
}
