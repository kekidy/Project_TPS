using UnityEngine;
using System.Collections;

public abstract class ElementalBullet : MonoBehaviour {
    [SerializeField] private ElementalType m_elementalType = ElementalType.NON;
    [Range(0f, 100f)]
    [SerializeField] private float   m_chargingRatePerOneShot = 0f;
    [SerializeField] private float   m_effectSecondsDuration  = 0f;
    [SerializeField] private Vector3 m_effectOffset           = Vector3.zero;

    private GameObject m_gameObject = null;

    public float   EffectSecondsDuration { get { return m_effectSecondsDuration; } }
    public Vector3 EffectOffset          { get { return m_effectOffset;          } }
    public bool    IsActivated           { get { return m_gameObject.activeSelf; } set { m_gameObject.SetActive(value); } }

    void Awake()
    {
        m_gameObject = gameObject;
        m_gameObject.SetActive(false);
        IsActivated = false;
    }

    public void OnEffect(RunnerBotCtrl enemy)
    {
        if (!enemy.IsOnCondition)
        {
            enemy.IncreaseElementalAccrue(m_chargingRatePerOneShot, m_elementalType);

            if (enemy.ElementalAccrue[(int)m_elementalType] >= 100f)
            {
                enemy.ResetElementalAccure();
                enemy.IsOnCondition = true;

                OnEvent(enemy);
            }
        }
    }

    public void SetActive(bool isActive)
    {
        m_gameObject.SetActive(isActive);
    }

    protected abstract void OnEvent(RunnerBotCtrl enemy);
}
