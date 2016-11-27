using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class DialogueTrigger : MonoBehaviour {
    [SerializeField] private GameObject m_triggerTarget     = null;
    [SerializeField] private float      m_startDelaySeconds = 0f;

    void OnDialogueMessage()
    {
        DialogueManager.Instance.OnDialogueMessage();
        Destroy(gameObject);
    }

    void OnTriggerStay(Collider col)
    {
        if ((col.gameObject == m_triggerTarget) && !DialogueManager.Instance.IsPlaying)
        {
            Invoke("OnDialogueMessage", m_startDelaySeconds);
            GetComponent<BoxCollider>().enabled = false;
        }   
    }
}
