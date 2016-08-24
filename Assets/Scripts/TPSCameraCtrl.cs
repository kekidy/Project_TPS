using UnityEngine;
using System.Collections;

public class TPSCameraCtrl : MonoBehaviour {
    [Header("TargetTraceInfo")]
    [SerializeField] private Transform m_traceTargetTransform = null;
    [SerializeField] private Vector3   m_distanceOffset       = Vector3.zero;

    [Header("Angle")]
    [SerializeField] private float     m_limitUpVerticalAngle   = 0f;
    [SerializeField] private float     m_limitDownVerticalAngle = 0f;

    [Header("Zoom")]
    [SerializeField] private float     m_zoomViewValue          = 30f;
    [SerializeField] private float     m_zoomCompleteSeconds    = 1f;

    private Animator  m_ownerAnim   = null;
    private Transform m_myTransform = null;
    private Camera    m_myCamera    = null;

    private float m_rotX = 0f;
    private float m_rotY = 0f;
    private float m_normalViewValue = 0f;

    private bool m_isZoomOn = false;

	void Awake () {
        m_myTransform     = transform;
        m_myCamera        = GetComponent<Camera>();
        m_ownerAnim       = m_traceTargetTransform.GetComponent<Animator>();
        m_normalViewValue = m_myCamera.fieldOfView;
	}

	void Update () {
        AngleCalculate();

        CameraZoomUpdate();
	}

    void OnDrawGizmos()
    {
        if (m_traceTargetTransform != null && m_myTransform != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(m_myTransform.position, m_traceTargetTransform.position);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(m_myTransform.position, m_myTransform.position + m_myTransform.forward * 5f);
            Gizmos.color = Color.white;
        }
    }

    private void AngleCalculate()
    {
        m_rotY += Input.GetAxis("Mouse X");
        m_rotX = Mathf.Clamp(m_rotX += -Input.GetAxis("Mouse Y"), m_limitDownVerticalAngle, m_limitUpVerticalAngle);

        m_myTransform.eulerAngles = new Vector3(m_rotX, m_rotY, 0.0f);
        m_myTransform.position = m_traceTargetTransform.position + m_myTransform.rotation * m_distanceOffset;

        float myAngleY     = m_myTransform.rotation.eulerAngles.y;
        float targetAngleY = m_traceTargetTransform.rotation.eulerAngles.y;
        float myRevisionAngleY     = myAngleY > 180 ? myAngleY - 360f : myAngleY;
        float targetRevisionAngleY = targetAngleY > 180 ? targetAngleY - 360f : targetAngleY;
        float _signedAngle = myRevisionAngleY - targetRevisionAngleY;

        //Vector3 forwardA = transform.rotation * Vector3.forward;
        //Vector3 forwardB = target.rotation * Vector3.forward;

        //float angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
        //float angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

        //float _signedAngle = Mathf.DeltaAngle(angleA, angleB);

        m_ownerAnim.SetFloat("horAngle", _signedAngle);
        m_ownerAnim.SetFloat("verAngle", m_rotX);
    }

    private void CameraZoomUpdate()
    {
        if (Input.GetButtonDown("Zoom"))
        {
            if (m_isZoomOn)
                m_isZoomOn = false;
            else
                m_isZoomOn = true;

            StopCoroutine("CameraZoom");
            StartCoroutine("CameraZoomOn", m_isZoomOn);
        }        

    }

    public IEnumerator CameraZoomOn(bool isZoomOn)
    {
        float zoomProgress = 0f;
        if (isZoomOn)
            zoomProgress = (m_normalViewValue - m_myCamera.fieldOfView) / (m_normalViewValue - m_zoomViewValue);
        else
            zoomProgress = 1f + ((m_myCamera.fieldOfView - m_normalViewValue) / (m_normalViewValue - m_zoomViewValue));
        
        while (true)
        {
            zoomProgress += Time.smoothDeltaTime;

            if (isZoomOn)
                m_myCamera.fieldOfView = Mathf.Lerp(m_normalViewValue, m_zoomViewValue, (zoomProgress / m_zoomCompleteSeconds));
            else
                m_myCamera.fieldOfView = Mathf.Lerp(m_zoomViewValue + (m_normalViewValue * zoomProgress), m_normalViewValue, (zoomProgress / m_zoomCompleteSeconds));

            if ((zoomProgress / m_zoomCompleteSeconds) >= 1f)
                break;

            yield return null;
        }
    }
}
