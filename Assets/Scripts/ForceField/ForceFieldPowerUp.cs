using UnityEngine;
using System.Collections;
using EasyEditor;

public class ForceFieldPowerUp : ForceFieldGauge {
    [Inspector(group = "Skill Data")]
    [SerializeField] private float m_powerUpMutiple = 1f;

    public override void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillActivate(playerCtrl);

        UseSkillGauge(playerCtrl, true);
    }

    public override void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        base.OnSkillDeactivate(playerCtrl);

        UseSkillGauge(playerCtrl, false);
    }
}
