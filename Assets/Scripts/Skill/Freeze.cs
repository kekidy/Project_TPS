using UnityEngine;
using System.Collections;

public class Freeze : MonoBehaviour {
    private RunnerBotCtrl m_runnerBotCtrl  = null;
    private Material      m_freezeMaterial = null;
    private float         m_freezeDuration = 0f;

    public void Init(RunnerBotCtrl runnerBotCtrl, Material freezeMaterial, float freezeDuration)
    {
        m_runnerBotCtrl  = runnerBotCtrl;
        m_freezeMaterial = freezeMaterial;
        m_freezeDuration = freezeDuration;

        StartCoroutine("OnFreeze");
    }

    private IEnumerator OnFreeze()
    {
        Renderer renderer = m_runnerBotCtrl.GetComponentInChildren<Renderer>(); 
        Material[] currentMaterial = renderer.materials;
        Material[] materials = new Material[currentMaterial.Length + 1];
        currentMaterial.CopyTo(materials, 0);
        materials[materials.Length - 1] = new Material(m_freezeMaterial); 
        renderer.materials = materials;

        Animator animator = m_runnerBotCtrl.GetComponent<Animator>();
        animator.speed = 0f;

        float currentSeconds = 0f;
        Color originColor = materials[materials.Length - 1].GetColor("_Color");
        Color stancilColor = originColor;
        stancilColor.a = 0f;

        while (true)
        {
            currentSeconds += Time.smoothDeltaTime;

            Color color = Color.Lerp(originColor, stancilColor, (currentSeconds / m_freezeDuration));
            materials[materials.Length - 1].SetColor("_Color", color);

            if ((currentSeconds / m_freezeDuration) >= 1f)
                break;

            yield return null;
        }

        renderer.materials = currentMaterial;
        m_runnerBotCtrl.IsOnCondition = false;
        animator.speed = 1f;

        Destroy(this);
    }
}
