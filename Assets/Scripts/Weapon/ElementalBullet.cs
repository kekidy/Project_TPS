using UnityEngine;
using System.Collections;

public abstract class ElementalBullet : MonoBehaviour {
    [SerializeField] private ElementalType m_elementalType = ElementalType.NON;
    [Range(0f, 100f)]
    [SerializeField] private float m_chargingRatePerOneShot = 0f;
    [SerializeField] private float m_effectSecondsDuration  = 0f;
    
    public float EffectSecondsDuration { get { return m_effectSecondsDuration; } }

    void Awake()
    {
        gameObject.SetActive(false);
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

    protected abstract void OnEvent(RunnerBotCtrl enemy);
}
