using UnityEngine;
using System.Collections;
using System;

/**
 * @brief 불 속성의 총알. 타겟에게 ignite(스크립트) 컴포넌트를 추가시킴.
 */
public class FireBullet : ElementalBullet {
    [SerializeField] private float      m_tickDamage      = 0;
    [SerializeField] private GameObject m_igniteEffectObj = null;

    protected override void OnEvent(RunnerBotCtrl enemy)
    {
        Ignite ignite = enemy.gameObject.AddComponent<Ignite>();
        ignite.Init(enemy, m_tickDamage, (int)EffectSecondsDuration);

        var obj = Instantiate(m_igniteEffectObj, enemy.transform) as GameObject;
        obj.transform.localPosition = EffectOffset;
        Destroy(obj, EffectSecondsDuration);
    }
}
