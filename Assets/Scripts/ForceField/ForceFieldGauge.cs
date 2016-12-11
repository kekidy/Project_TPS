using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EasyEditor;

/**
 * @brief 스킬 게이지를 사용하는 모든 스킬들이 상속받는 베이스 클래스
 */

public abstract class ForceFieldGauge : ForceFieldSkill {
    [Inspector(group = "Skill Gauge Info")]
    [SerializeField] private Image m_skillGaugeImage     = null;
    [SerializeField] private Color m_skillGaugeColor     = Color.white;
    [SerializeField] private float m_skillRatePerSeconds = 0f;

    protected void UseSkillGauge(PlayerCtrl playerCtrl, bool isUse)
    {
        if (isUse)
        {
            m_skillGaugeImage.color = m_skillGaugeColor;

            StopCoroutine("GaugeChange");
            StartCoroutine("GaugeChange", playerCtrl);
        }
        else
            StopCoroutine("GaugeChange");
    }

    private IEnumerator GaugeChange(PlayerCtrl playerCtrl)
    {
        while (true)
        {
            yield return null;

            m_skillGaugeImage.fillAmount -= ((m_skillRatePerSeconds * Time.smoothDeltaTime) / playerCtrl.MaxSkillGauge);
            if (m_skillGaugeImage.fillAmount <= 0f)
               break;    
        }

        IsActivated = false;
    }
}
