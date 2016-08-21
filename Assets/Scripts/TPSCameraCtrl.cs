using UnityEngine;
using System.Collections;

public class TPSCameraCtrl : MonoBehaviour {
    [SerializeField] private Transform m_ownerTransform;
    [SerializeField] private Vector3   m_distanceOffset = Vector3.zero;
    [SerializeField] private float     m_limitUpVerticalAngle   = 0f;
    [SerializeField] private float     m_limitDownVerticalAngle = 0f;

    private Animator  m_ownerAnim;

    private Transform m_myTransform;

    private float m_rotX;
    private float m_rotY;

	void Awake () {
        m_myTransform    = transform;
        m_ownerAnim      = m_ownerTransform.GetComponent<Animator>();
	}

	void Update () {
        AngleCalculate();
	}

    void OnDrawGizmos()
    {
        if (m_ownerTransform != null && m_myTransform != null)
        {
            Gizmos.DrawLine(m_myTransform.position, m_ownerTransform.position + new Vector3(0f, m_distanceOffset.y, 0f));
            Gizmos.DrawLine(m_myTransform.position, m_myTransform.position + m_myTransform.forward * 5f);
        }
    }

    private void AngleCalculate()
    {
        m_rotY += Input.GetAxis("Mouse X");
        m_rotX = Mathf.Clamp(m_rotX += -Input.GetAxis("Mouse Y"), m_limitDownVerticalAngle, m_limitUpVerticalAngle);

        m_myTransform.eulerAngles = new Vector3(m_rotX, m_rotY, 0.0f);
        m_myTransform.position = m_ownerTransform.position + m_myTransform.rotation * m_distanceOffset;

        float angleA = m_myTransform.rotation.eulerAngles.y > 180 ? m_myTransform.rotation.eulerAngles.y - 360f : m_myTransform.rotation.eulerAngles.y;
        float angleB = m_ownerTransform.rotation.eulerAngles.y > 180 ? m_ownerTransform.rotation.eulerAngles.y - 360f : m_ownerTransform.rotation.eulerAngles.y;
        float _signedAngle = angleA - angleB;

        //Vector3 forwardA = transform.rotation * Vector3.forward;
        //Vector3 forwardB = target.rotation * Vector3.forward;

        //float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
        //float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

        //float _signedAngle = Mathf.DeltaAngle(angleA, angleB);

        m_ownerAnim.SetFloat("horAngle", _signedAngle);
        m_ownerAnim.SetFloat("verAngle", m_rotX);
    }
}
