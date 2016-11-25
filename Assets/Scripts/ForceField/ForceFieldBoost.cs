using UnityEngine;
using EasyEditor;
using System.Collections;
using System;
using UnityStandardAssets.ImageEffects;

public class ForceFieldBoost : ForceFieldGauge {
    [Inspector(group = "Skill Data")]
    [SerializeField] private float m_speedMutiple = 1f;
    
    [Inspector(group = "FX Info")]
    [Header("Blur")]
    [SerializeField] private CameraMotionBlur m_cameraMotionBlur = null;
    [Header("Psycho")]
    [SerializeField] private CameraFilterPack_Vision_Psycho m_psychoEffect = null;
    [SerializeField] private float m_aniamteSeconds = 0f;
    [SerializeField] private float m_maxAmount      = 0f;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);
        playerCtrl.Speed = m_speedMutiple;

        m_cameraMotionBlur.enabled = true;
        m_psychoEffect.enabled     = true;

        StartCoroutine("PsychoAnimate");

        UseSkillGauge(playerCtrl, true);
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);
        playerCtrl.Speed = 1f;

        m_cameraMotionBlur.enabled = false;
        m_psychoEffect.enabled     = false;

        StopCoroutine("PsychoAnimate");

        UseSkillGauge(playerCtrl, false);
    }

    private IEnumerator PsychoAnimate()
    {
        float currentSeconds = 0f;

        while (true)
        {
            currentSeconds += Time.smoothDeltaTime;

            float lerpValue = (currentSeconds / m_aniamteSeconds);

            m_psychoEffect.HoleSmooth = Mathf.Lerp(0f, m_maxAmount, lerpValue);

            if (lerpValue >= 1f)
                break;

            yield return null;
        }
    }
}
