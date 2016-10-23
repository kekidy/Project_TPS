using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;

public abstract class ForceFieldSkill : MonoBehaviour {
    private Image m_iconImage = null;

    public bool IsActivated { get; set; }

    void Awake()
    {
        m_iconImage = GetComponent<Image>();
        m_iconImage.enabled = false;
    }

    public virtual void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        Debug.Assert(playerCtrl != null, (GetType().Name + "::OnSkillActivate - player is null"));

        m_iconImage.enabled = true;
        IsActivated = true;
    }

    public virtual void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        Debug.Assert(playerCtrl != null, (GetType().Name + "::OnSkillActivate - player is null"));

        m_iconImage.enabled = false;
        IsActivated = false;
    }
}
