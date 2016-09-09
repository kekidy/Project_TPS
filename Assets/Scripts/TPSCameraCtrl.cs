using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;

public struct RaycastHitInfomation
{
    public bool       isHit;
    public RaycastHit rayHit;

    public RaycastHitInfomation(bool _isHit, RaycastHit _rayHit = new RaycastHit())
    {
        isHit  = _isHit;
        rayHit = _rayHit;
    }
}

public class TPSCameraCtrl : MonoBehaviour {
    public static TPSCameraCtrl Instance { get; private set; }

    [Header("TargetTraceInfo")]
    [SerializeField] private Transform m_traceTargetTransform = null;
    [SerializeField] private Vector3   m_distanceOffset       = Vector3.zero;
    [SerializeField] private Transform m_targetHeadTransform  = null;

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

    public RaycastHitInfomation CameraCenterRaycast
    {
        get
        {
            Vector3 rayPos = m_myCamera.ScreenToWorldPoint(Vector3.zero);
            Vector3 rayDir = m_myTransform.forward;
            RaycastHit rayHit;

            if (Physics.Raycast(rayPos, rayDir, out rayHit, Mathf.Infinity))
                return new RaycastHitInfomation(true, rayHit);
            else
                return new RaycastHitInfomation(false);
        }
    }

	void Awake () {
        Instance          = this;
        m_myTransform     = transform;
        m_myCamera        = GetComponent<Camera>();
        m_ownerAnim       = m_traceTargetTransform.GetComponent<Animator>();
        m_normalViewValue = m_myCamera.fieldOfView;
	}

	void Update () {
        AngleCalculate();
        CameraZoomUpdate();
    }

    void LateUpdate()
    {
        SpringArmUpdate();
    }

    private void AngleCalculate()
    {
        if (!m_ownerAnim.GetBool("isVault"))
        {
            m_rotY += Input.GetAxis("Mouse X");
            m_rotX = Mathf.Clamp(m_rotX += -Input.GetAxis("Mouse Y"), m_limitDownVerticalAngle, m_limitUpVerticalAngle);
        }

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
            SetCameraZoom(!m_isZoomOn);
    }

    public void SetCameraZoom(bool isZoomOn)
    {
        m_isZoomOn = isZoomOn;
        StopCoroutine("CameraZoom");
        StartCoroutine("CameraZoomOn", m_isZoomOn);
    }

    private void SpringArmUpdate()
    {
        RaycastHit hit;
        if (Physics.Linecast(m_targetHeadTransform.position, m_myTransform.position, out hit))
            m_myTransform.position = hit.point;
    }

    private IEnumerator CameraZoomOn(bool isZoomOn)
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
