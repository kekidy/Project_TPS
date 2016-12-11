using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/**
 * @brief Light가 전력 부족인것처럼 깜빡거리는 효과를 주는 스크립트
 */
[RequireComponent(typeof(Light))]
public class LightFilker : MonoBehaviour {
    [SerializeField] private float m_coreBase  = 0f;
    [SerializeField] private float m_amplitude = 0f;

    private Light m_myLight   = null;
    private Color m_mainColor = Color.white;

    public float CoreBase  { get { return m_coreBase;  } set { m_coreBase = value;  } }
    public float Amplitude { get { return m_amplitude; } set { m_amplitude = value; } }

	void Awake () {
        m_myLight   = GetComponent<Light>();
        m_mainColor = m_myLight.color;

        this.UpdateAsObservable()
            .Select(_ => 1f - (Random.value * 2f))
            .Select(offset => (offset * m_amplitude) + m_coreBase)
            .Subscribe(colorMultiple => m_myLight.color = (m_mainColor * colorMultiple));
    }
}
