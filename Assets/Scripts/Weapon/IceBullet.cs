using UnityEngine;
using System.Collections;

/**
 * @brief 얼음 속성의 총알. 타겟에게 Freeze(스크립트) 컴포넌트를 추가시킴.
 */
public class IceBullet : ElementalBullet {
    [SerializeField] private Material   m_freezeMaterial = null;
    [SerializeField] private GameObject m_smokeEffectObj = null;

    protected override void OnEvent(RunnerBotCtrl enemy)
    {
        Freeze freeze = enemy.gameObject.AddComponent<Freeze>();
        freeze.Init(enemy, m_freezeMaterial, EffectSecondsDuration);

        var obj = Instantiate(m_smokeEffectObj, enemy.transform) as GameObject;
        obj.transform.localPosition = EffectOffset;
        Destroy(obj, EffectSecondsDuration);
    }
}
