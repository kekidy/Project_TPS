using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using EasyEditor;

/**
 * @brief TPS 시점 카메라 제어 클래스
 */
public class TPSCameraCtrl : MonoBehaviour {
    public static TPSCameraCtrl Instance { get; private set; }

    [Inspector(group = "Target Trace Info")]
    [SerializeField] private Transform m_traceTargetTransform = null;         //! [SerializeField] 카메라의 추적 대상이 되는 GameObject의 Transform (Player GameObject)
    [SerializeField] private Vector3   m_distanceOffset       = Vector3.zero; //! [SerializeField] 추적 대상과의 거리 offset 값
    [SerializeField] private Transform m_targetHeadTransform  = null;         //! [SerializeField] Spring Arm 생성을 위해 Linecast로 카메라와 이어질 기준 위치의 Transform 

    [Inspector(group = "Angle")]
    [SerializeField] private float     m_limitUpVerticalAngle   = 0f;         //! [SerializeField] 카메라 오브젝트의 Vertical 축 +-제한 각도
    [SerializeField] private float     m_limitDownVerticalAngle = 0f;         //! [SerializeField] 카메라 오브젝트의 Horizontal 축 +-제한 각도

    [Inspector(group = "Zoom")]
    [SerializeField] private float     m_zoomViewValue          = 30f;        //! [SerializeField] 카메라 줌시 시야각
    [SerializeField] private float     m_zoomCompleteSeconds    = 1f;         //! [SerializeField] 카메라 줌 애니메이트 시간

    private Animator   m_ownerAnim    = null; //! 카메라 추적중인 대상의 Animator
    private Transform  m_myTransform  = null; //! 카메라 오브젝트의 transform 캐싱용 변수
    private Camera     m_myCamera     = null; //! 카메라 오브젝트의 Camera 컴포넌트 캐싱용 변수
    private RaycastHit m_cameraCenterrayHit;  //! 카메라의 Center에서 발사한 Ray에 부딛친 Hit 오브젝트의 정보를 저장
    private RaycastHit m_targetRayHit;        //! Linecast로 Spring Arm에 부딛친 Hit 오브젝트의 정보를저장 

    private float m_rotX = 0f;
    private float m_rotY = 0f;
    private float m_normalViewValue = 0f;

    private bool m_isZoomOn = false;
    private bool m_isRayHit = false;

    public bool ZoomOn
    {
        get { return m_isZoomOn; }
        set
        {
            m_isZoomOn = value;
            StopCoroutine("CameraZoom");
            StartCoroutine("CameraZoomOn", m_isZoomOn);
        }
    }

    public RaycastHit RayHit   { get { return m_cameraCenterrayHit;   } }
    public bool       IsRayHit { get { return m_isRayHit; } }

	void Awake () {
        Instance          = this;
        m_myTransform     = transform;
        m_myCamera        = GetComponent<Camera>();
        m_ownerAnim       = m_traceTargetTransform.GetComponent<Animator>();
        m_normalViewValue = m_myCamera.fieldOfView;
        m_myTransform.eulerAngles = new Vector3(0f, 0f, 0f);
    }

	void Start () {
        AngleCalculateObservable();
        CameraZoomUpdateObservable();
        CameraCenterRaycastObservable();
        TraceToTargetLaterObservable();
        SpringArmLaterObservable();
    }

    private void CameraCenterRaycastObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Subscribe(_ => {
                Vector3 rayPos = m_myCamera.ScreenToWorldPoint(Vector3.zero);
                Vector3 rayDir = m_myTransform.forward;
                m_isRayHit = Physics.Raycast(rayPos, rayDir, out m_cameraCenterrayHit, Mathf.Infinity);
            });
    }

    private void AngleCalculateObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => !m_ownerAnim.GetBool("isVault"))
            .Where(_ => Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            .Subscribe(_ => {
                m_rotY += Input.GetAxis("Mouse X");
                m_rotX = Mathf.Clamp(m_rotX += -Input.GetAxis("Mouse Y"), m_limitDownVerticalAngle, m_limitUpVerticalAngle);
                m_myTransform.eulerAngles = new Vector3(m_rotX, m_rotY, 0.0f);

                float myAngleY = m_myTransform.rotation.eulerAngles.y;
                float targetAngleY = m_traceTargetTransform.rotation.eulerAngles.y;
                float myRevisionAngleY = myAngleY > 180 ? myAngleY - 360f : myAngleY;
                float targetRevisionAngleY = targetAngleY > 180 ? targetAngleY - 360f : targetAngleY;
                float _signedAngle = myRevisionAngleY - targetRevisionAngleY;

                m_ownerAnim.SetFloat("horAngle", _signedAngle);
                m_ownerAnim.SetFloat("verAngle", m_rotX);
            });
    }

    private void TraceToTargetLaterObservable()
    {
        this.LateUpdateAsObservable()
            .Where(_ => this.enabled)
            .Select(_ => m_ownerAnim.GetBool("isVault"))
            .Subscribe(isVault => {
                if (isVault)
                {
                    Vector3 alivePos = m_traceTargetTransform.position + m_myTransform.rotation * m_distanceOffset;
                    alivePos.y = m_myTransform.position.y;
                    m_myTransform.position = alivePos;
                }
                else
                    m_myTransform.position = m_traceTargetTransform.position + m_myTransform.rotation * m_distanceOffset;
            });
    }

    private void CameraZoomUpdateObservable()
    {
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => Input.GetButtonDown("Zoom"))
            .Subscribe (_ => ZoomOn = !m_isZoomOn);
    }

    private void SpringArmLaterObservable()
    {
        this.LateUpdateAsObservable()
            .Where(_ => this.enabled)
            .Where(_ => Physics.Linecast(m_targetHeadTransform.position, m_myTransform.position, out m_targetRayHit))
            .Subscribe(_ => m_myTransform.position = m_targetRayHit.point + (m_targetRayHit.normal * 0.05f));
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
