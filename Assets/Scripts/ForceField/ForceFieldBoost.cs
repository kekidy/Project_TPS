using UnityEngine;
using EasyEditor;
using System.Collections;
using System;
using UnityStandardAssets.ImageEffects;

public class ForceFieldBoost : ForceFieldGauge {
    [Inspector(group = "Skill Data")]
    [SerializeField] private float m_speedMutiple = 1f;
    [SerializeField] private CameraMotionBlur m_cameraMotionBlur = null;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);
        playerCtrl.Speed = m_speedMutiple;

        m_cameraMotionBlur.enabled = true;

        UseSkillGauge(playerCtrl, true);
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);
        playerCtrl.Speed = 1f;

        m_cameraMotionBlur.enabled = false;

        UseSkillGauge(playerCtrl, false);
    }
}
