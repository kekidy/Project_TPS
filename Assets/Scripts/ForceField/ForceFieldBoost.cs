using UnityEngine;
using EasyEditor;
using System.Collections;
using System;

public class ForceFieldBoost : ForceFieldGauge {
    [Inspector(group = "Skill Data")]
    [SerializeField] private float m_speedMutiple = 1f;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);
        playerCtrl.Speed = m_speedMutiple;

        UseSkillGauge(true);
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);
        playerCtrl.Speed = 1f;

        UseSkillGauge(false);
    }
}
