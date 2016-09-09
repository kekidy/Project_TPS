using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine.UI;

public class ScrollMinimapCtrl : MonoBehaviour {
    public static ScrollMinimapCtrl Instance { get; private set; }

    [SerializeField] private Transform     m_playerTransform         = null;
    [SerializeField] private RectTransform m_groupRectTansform       = null;
    [SerializeField] private RectTransform m_targetIconRectTransform = null;
    [SerializeField] private float m_angleMultiplicatinValue = 0f;
    
    private float m_negativeStartAngle = 0;
    private float m_prevTargetAngle    = 0f;

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
        m_prevTargetAngle = m_playerTransform.eulerAngles.y;

        MinimapScrollObserve();
        SetTargetAngle(90);
    }

    private void MinimapScrollObserve()
    {
        this.LateUpdateAsObservable()
            .Select(_ => m_playerTransform.eulerAngles.y)
            .DistinctUntilChanged()
            .Subscribe(angle => m_groupRectTansform.localPosition = AngleToLocalPosition(angle, true));
    }

    private void SetTargetAngle(float angle)
    {
        m_targetIconRectTransform.localPosition = AngleToLocalPosition(angle, false, m_targetIconRectTransform.localPosition.y);
    }

    private Vector3 AngleToLocalPosition(float angle, bool isFlip = false, float posY = 0f)
    {
        float rivisionAngle = angle > 180f ? (angle - 360f) : angle;
        int flip = isFlip ? -1 : 1;

        return rivisionAngle >= 0 ? new Vector3(rivisionAngle * flip * m_angleMultiplicatinValue, posY, 0f)
            : new Vector3(m_negativeStartAngle + (rivisionAngle * flip * m_angleMultiplicatinValue), posY, 0f);
    }
}
