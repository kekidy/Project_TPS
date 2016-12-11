using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/**
 * @brief 지정한 대상이 Trigger Box에 닿으면 지정한 이벤트를 실행시키는 이벤트 트리거 스크립트
 */

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour {
    [SerializeField] private GameObject m_eventTarget        = null;
    [SerializeField] private float      m_startDelaySeconds  = 0f;
    [SerializeField] private UnityEvent m_onEvent            = null;
    [SerializeField] private bool       m_autoDestroy        = true;

    void OnEvent()
    {
        m_onEvent.Invoke();

        if (m_autoDestroy)
            Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == m_eventTarget)
        {
            Invoke("OnEvent", m_startDelaySeconds);
        }
    }
}
