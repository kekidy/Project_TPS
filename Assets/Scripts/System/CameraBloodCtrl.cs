using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/**
 * @brief 피격시 발생하는 셰이더를 제어하는 컨트롤러 스크립트
 */

public class CameraBloodCtrl : MonoBehaviour {
    public static CameraBloodCtrl Instance { get; private set; }

    [SerializeField] private CameraFilterPack_Vision_Blood m_visionBlood = null;
    [SerializeField] private float m_startBloodSize  = 0f;
    [SerializeField] private float m_durationSeconds = 0f;

    private float m_currentSeconds = 0f;

    void Awake()
    {
        Instance = this;

        this.UpdateAsObservable()
            .Where(_ => m_visionBlood.enabled)
            .Subscribe(_ => {
                m_currentSeconds += Time.smoothDeltaTime;
                m_visionBlood.HoleSize = Mathf.Lerp(m_startBloodSize, 1f, m_currentSeconds / m_durationSeconds);

                if ((m_currentSeconds / m_durationSeconds) >= 1.0f)
                    m_visionBlood.enabled = false;
            });
    }

    public void OnBloodEffect()
    {
        m_currentSeconds      = 0f;
        m_visionBlood.enabled = true;
    }
}
