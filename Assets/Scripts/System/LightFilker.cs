using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

[RequireComponent(typeof(Light))]
public class LightFilker : MonoBehaviour {
    [SerializeField] private float m_coreBase  = 0f;
    [SerializeField] private float m_amplitude = 0f;

    private Light m_myLight   = null;
    private Color m_mainColor = Color.white;

	void Awake () {
        m_myLight   = GetComponent<Light>();
        m_mainColor = m_myLight.color;

        this.UpdateAsObservable()
            .Select(_ => 1f - (Random.value * 2f))
            .Select(offset => (offset * m_amplitude) + m_coreBase)
            .Subscribe(colorMultiple => m_myLight.color = (m_mainColor * colorMultiple));
    }
}
