using UnityEngine;
using System.Collections;

/**
 * @brief 지정한 대상이 Trigger Box에 닿으면 각종 대사들을 진행 시키는 트리거 스크립트
 */

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
