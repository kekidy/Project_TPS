using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using EasyEditor;

public abstract class ForceFieldGauge : ForceFieldSkill {
    [Inspector(group = "Skill Gauge Info")]
    [SerializeField] private Image m_skillGaugeImage = null;
    [SerializeField] private Color m_skillGaugeColor = Color.white;
    [SerializeField] private float m_skillUseSeconds = 1f;

    protected void UseSkillGauge(bool isUse)
    {
        if (isUse)
        {
            m_skillGaugeImage.color = m_skillGaugeColor;

            StopCoroutine("GaugeChange");
            StartCoroutine("GaugeChange");
        }
        else
            StopCoroutine("GaugeChange");
    }

    private IEnumerator GaugeChange()
    {
        while (true)
        {
            yield return null;

            m_skillGaugeImage.fillAmount -= (Time.smoothDeltaTime / m_skillUseSeconds);
            if (m_skillGaugeImage.fillAmount <= 0f)
               break;    
        }

        IsActivated = false;
    }
}
