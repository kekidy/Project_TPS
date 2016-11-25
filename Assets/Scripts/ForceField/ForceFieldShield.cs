using UnityEngine;
using EasyEditor;
using System.Collections;

public class ForceFieldShield : ForceFieldSkill {
    [Inspector(group = "FX Info")]
    [SerializeField] private CameraFilterPack_AAA_SuperHexagon m_superHexagon = null;
    [SerializeField] private float m_animateSeconds = 0f;
    [SerializeField] private float m_maxAmount      = 0f;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);

        m_superHexagon.enabled = true;
        StartCoroutine("SuperHexagonAnimate");
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);

        m_superHexagon.enabled = false;
        StopCoroutine("SuperHexagonAnimate");
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
