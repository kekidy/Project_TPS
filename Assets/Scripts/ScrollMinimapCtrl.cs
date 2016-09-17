using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using UnityEngine.UI;

public class ScrollMinimapCtrl : MonoBehaviour {
    public static ScrollMinimapCtrl Instance { get; private set; }

    [SerializeField] private Transform     m_playerTransform         = null;
    [SerializeField] private RectTransform m_angleScrollerRectTansform       = null;
    [SerializeField] private RectTransform m_targetIconRectTransform = null;
    [SerializeField] private float m_angleMultiplicatinValue = 0f;
    
    public Transform testPosition;

    private float m_negativeStartAngle = 0;

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
        this.UpdateAsObservable()
            .Subscribe(_ => {
                float angle = Vector3.Angle(m_playerTransform.forward, testPosition.position - m_playerTransform.position);
                Vector3 cross = Vector3.Cross(m_playerTransform.forward, testPosition.position - m_playerTransform.position);
                float rivisionAngle = cross.y < 0 ? -angle : angle;

                m_targetIconRectTransform.localPosition = new Vector3(rivisionAngle * m_angleMultiplicatinValue, 0f, 0f);
            });
    }

    private Vector3 AngleToLocalPosition(float angle, bool isFlip = false, float posY = 0f)
    {
        float rivisionAngle = angle > 180f ? (angle - 360f) : angle;
        int flip = isFlip ? -1 : 1;
        return new Vector3(rivisionAngle * flip * m_angleMultiplicatinValue, posY, 0f);
    }
}
