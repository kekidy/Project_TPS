using UnityEngine;
using System.Collections;

public class IceBullet : ElementalBullet {
    [SerializeField] private Material   m_freezeMaterial = null;
    [SerializeField] private GameObject m_smokeEffectObj = null;

    protected override void OnEvent(RunnerBotCtrl enemy)
    {
        Freeze freeze = enemy.gameObject.AddComponent<Freeze>();
        freeze.Init(enemy, m_freezeMaterial, EffectSecondsDuration);

        var obj = Instantiate(m_smokeEffectObj, enemy.transform) as GameObject;
        obj.transform.localPosition = new Vector3(0f, 1f, 0f);
        Destroy(obj, EffectSecondsDuration);
    }
}
