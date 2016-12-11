using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine.UI;

/**
 * @brief 이동해야하는 목표 지점의 방향을 가르쳐주는 ScrollMinimap을 제어하는 컨트롤러 클래스
 */

public class ScrollMinimapCtrl : MonoBehaviour {
    public static ScrollMinimapCtrl Instance { get; private set; }

    [SerializeField] private Transform     m_playerTransform         = null;
    [SerializeField] private RectTransform m_targetIconRectTransform = null;
    [SerializeField] private float m_angleMultiplicatinValue = 0f;

    private Vector3 m_targetPoint = Vector3.zero;
    private float m_negativeStartAngle = 0;

    public Vector3 TargetPoint { set { m_targetPoint = value; } }

	void Awake () {
        if (Instance)
        {
            Destroy(gameObject);
            Debug.LogWarning("ScroellMinimapCtrl is Exist");
            return;
        }

        Instance = this;
        m_negativeStartAngle = 20f * m_angleMultiplicatinValue;
	}

    void Start()
    {
        MinimapScrollObserve();
    }

    private void MinimapScrollObserve()
    {
        this.ObserveEveryValueChanged(_ => m_playerTransform.position)
            .Subscribe(_ => {
                float angle = Vector3.Angle(m_playerTransform.forward, m_targetPoint - m_playerTransform.position);
                Vector3 cross = Vector3.Cross(m_playerTransform.forward, m_targetPoint - m_playerTransform.position);
                float rivisionAngle = cross.y < 0 ? -angle : angle;

                m_targetIconRectTransform.localPosition = new Vector3(rivisionAngle * m_angleMultiplicatinValue, 0f, 0f);
            });
    }
}
