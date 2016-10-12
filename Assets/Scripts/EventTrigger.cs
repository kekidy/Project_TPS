using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class EventTrigger : MonoBehaviour {
    [SerializeField] private GameObject m_eventTarget = null;
    [SerializeField] private float      m_startDelaySeconds  = 0f;
    [SerializeField] private UnityEvent m_onEvent     = null;

    void OnEvent()
    {
        m_onEvent.Invoke();
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == m_eventTarget)
        {
            Invoke("OnEvent", m_startDelaySeconds);
            GetComponent<BoxCollider>().enabled = false;
        }
    }
}
