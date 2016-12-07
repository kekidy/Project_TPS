using UnityEngine;
using System.Collections;

public class Ignite : MonoBehaviour {
    private RunnerBotCtrl  m_runnerBotCtrl  = null;
    private float          m_tickDamage     = 0f;
    private int            m_igniteDuration = 0;

    private WaitForSeconds m_tickSeconds    = new WaitForSeconds(1.0f); 

    public void Init(RunnerBotCtrl runnerBotCtrl, float tickDamage, int igniteDuration)
    {
        m_runnerBotCtrl  = runnerBotCtrl;
        m_tickDamage = tickDamage;
        m_igniteDuration = igniteDuration;

        StartCoroutine("OnIgnite");
    }

    private IEnumerator OnIgnite()
    {
        for (int i = 0; i < m_igniteDuration; i++)
        {
            yield return m_tickSeconds;
            m_runnerBotCtrl.BeAttacked(m_tickDamage);
        }

        Destroy(this);
    }
}
