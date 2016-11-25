using UnityEngine;
using EasyEditor;

public abstract class ForceFieldSkill : MonoBehaviour {


    private GameObject m_iconObj = null;

    public bool IsActivated { get; set; }

    void Awake()
    {
        m_iconObj = gameObject;
        m_iconObj.SetActive(false);
    }

    public virtual void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        m_iconObj.SetActive(true);
        IsActivated = true;
    }

    public virtual void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        m_iconObj.SetActive(false);
        IsActivated = false;
    }
}
