using UnityEngine;
using System.Collections;
using EasyEditor;

/**
 * @brief 플레이어블 캐릭터의 데미지를 올려주는 스킬 스크립트
 */

public class ForceFieldPowerUp : ForceFieldGauge {
    [Inspector(group = "Skill Data")]
    [SerializeField] private float m_powerUpMutiple = 1f;

    [Inspector(group = "FX Info")]
    [SerializeField] private CameraFilterPack_AAA_SuperComputer m_superComputer = null;
    [SerializeField] private float m_animateSeconds = 0f;
    [SerializeField] private float m_maxAmount      = 0f;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);

        m_superComputer.enabled = true;
        StartCoroutine("SuperComputerAnimate");

        UseSkillGauge(playerCtrl, true);
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);

        m_superComputer.enabled = false;
        StopCoroutine("SuperComputerAnimate");

        UseSkillGauge(playerCtrl, false);
    }

    private IEnumerator SuperComputerAnimate()
    {
        float currentSeconds = 0f;

        while (true)
        {
            currentSeconds += Time.smoothDeltaTime;

            float lerpValue = (currentSeconds / m_animateSeconds);

            m_superComputer._AlphaHexa = Mathf.Lerp(0f, m_maxAmount, lerpValue);

            if (lerpValue >= 1f)
                break;

            yield return null;
        }
    }
}
