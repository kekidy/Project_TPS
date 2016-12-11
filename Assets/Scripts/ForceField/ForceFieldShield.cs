using UnityEngine;
using EasyEditor;
using UnityEngine.UI;
using System.Collections;

/**
 * @brief 플레이어블 캐릭터의 쉴드를 활성화시키는 스킬 스크립트
 */

public class ForceFieldShield : ForceFieldSkill {
    [Inspector(group = "Skill Gauge Info")]
    [SerializeField] private Image m_shieldGaugeImage = null;

    [Inspector(group = "FX Info")]
    [Header("Active FX")]
    [SerializeField] private CameraFilterPack_AAA_SuperHexagon m_superHexagon = null;
    [SerializeField] private float m_animateSeconds    = 0f;
    [SerializeField] private float m_maxAmount         = 0f;

    [Header("Hit FX")]
    [SerializeField] private float m_originSpotSize    = 0f;
    [SerializeField] private float m_startSpotSize     = 0f;
    [SerializeField] private float m_hitAnimateSeconds = 0f;

    private float m_currentSeconds = 0f;
    private bool  m_isHitAnimating = false;

    void Update()
    {
        if (m_isHitAnimating)
        {
            m_currentSeconds += Time.smoothDeltaTime;
            m_superHexagon._SpotSize = Mathf.Lerp(m_startSpotSize, m_originSpotSize, m_currentSeconds / m_hitAnimateSeconds);

            if ((m_currentSeconds / m_hitAnimateSeconds) >= 1.0f)
                m_isHitAnimating = false;
        }
    }

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);

        playerCtrl.FFShield = this;

        m_superHexagon.enabled = true;
        StartCoroutine("SuperHexagonAnimate");
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);

        playerCtrl.FFShield = null;

        m_superHexagon.enabled = false;
        StopCoroutine("SuperHexagonAnimate");
    }

    public float OnPrtected(PlayerCtrl playerCtrl, float damage)
    {
        m_isHitAnimating = true;
        m_currentSeconds = 0f;

        float remainGauge = playerCtrl.CurrentShieldGauge - damage;
        playerCtrl.CurrentShieldGauge = remainGauge > 0f ? remainGauge : 0f;

        m_shieldGaugeImage.fillAmount = playerCtrl.CurrentShieldGauge / playerCtrl.MaxShieldGauge;

        if (m_shieldGaugeImage.fillAmount <= 0f)
            OnSkillDeactivate(playerCtrl);

        return remainGauge < 0f ? remainGauge : 0;
    }

    private IEnumerator SuperHexagonAnimate()
    {
        float currentSeconds = 0f;

        while (true)
        {
            currentSeconds += Time.smoothDeltaTime;

            float lerpValue = (currentSeconds / m_animateSeconds);

            m_superHexagon._AlphaHexa = Mathf.Lerp(0f, m_maxAmount, lerpValue);

            if (lerpValue >= 1f)
                break;

            yield return null;
        }
    }
}
