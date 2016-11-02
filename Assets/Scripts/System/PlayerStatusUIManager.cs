using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using EasyEditor;
using UniLinq;
using System.Collections;

public class PlayerStatusUIManager : MonoBehaviour {
    [Inspector(group = "Target Info")]
    [SerializeField] private PlayerCtrl m_playerCtrl = null;

    [Inspector(group = "Status Image Info")]
    [SerializeField] private Image m_shieldGaugeImage = null;
    [SerializeField] private Image m_skillGaugeImage  = null;

    [Inspector(group = "Skill Image Info")]
    [SerializeField] private Image   m_shiledImage     = null;
    [SerializeField] private Image[] m_skillImageArray = null;

    [Inspector(group = "Status Charge Speed Info")]
    [SerializeField] private float m_shieldRatePerSeconds = 0f;
    [SerializeField] private float m_skillRatePerSeconds  = 0f;

	void Start () {
        this.LateUpdateAsObservable()
            .Where(_ => m_shieldGaugeImage.fillAmount < 1f)
            .Where(_ => !m_shiledImage.enabled)
            .Subscribe(_ => m_shieldGaugeImage.fillAmount += ((Time.smoothDeltaTime * m_shieldRatePerSeconds) / m_playerCtrl.MaxShieldGauge));

        this.LateUpdateAsObservable()
            .Where(_ => m_skillGaugeImage.fillAmount < 1f)
            .Where(_ => m_skillImageArray.All(image => !image.enabled))
            .Subscribe(_ => m_skillGaugeImage.fillAmount += ((Time.smoothDeltaTime * m_skillRatePerSeconds) / m_playerCtrl.MaxSkillGauge));
	}
}
