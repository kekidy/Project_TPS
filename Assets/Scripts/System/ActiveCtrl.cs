using UnityEngine;
using System.Collections;

/**
 * @brief 특정 키를 누르면 지정한 오브젝트가 활성화 혹은 비활성화되게 해주는 스크립트
 */

public class ActiveCtrl : MonoBehaviour {
    [SerializeField] private GameObject m_activeObject = null;
    [SerializeField] private KeyCode    m_activeKey    = KeyCode.None;

	void Update () {
        if (Input.GetKey(m_activeKey))
            m_activeObject.SetActive(true);
        else
            m_activeObject.SetActive(false);
	}
}
