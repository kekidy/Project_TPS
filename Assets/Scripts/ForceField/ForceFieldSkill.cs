using UnityEngine;
using EasyEditor;

public abstract class ForceFieldSkill : MonoBehaviour {
    private GameObject m_gameObject = null;

    public bool IsActivated { get { return m_gameObject.activeSelf; }  set { m_gameObject.SetActive(value); } }

    void Awake()
    {
        m_gameObject = gameObject;
        m_gameObject.SetActive(false);
    }

    public virtual void OnSkillActivate(PlayerCtrl playerCtrl)
    {
        IsActivated = true;
    }

    public virtual void OnSkillDeactivate(PlayerCtrl playerCtrl)
    {
        IsActivated = false;
    }
}
