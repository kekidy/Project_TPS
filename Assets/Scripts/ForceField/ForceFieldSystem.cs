using UnityEngine;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using System.Collections;
using EasyEditor;
using UnityEngine.Events;

/**
 * @brief Force Field 스킬의 발동을 담당하는 시스템 스크립트
 */

[RequireComponent(typeof(AudioSource))]
public class ForceFieldSystem : MonoBehaviour {
    [System.Serializable]
    private class SkillData
    {
        public ForceFieldType skillType     = ForceFieldType.NON;
        public KeyCode        acitveKeyCode = KeyCode.None;

        public Color     innerColor    = Color.white;
        public Color     outerColor    = Color.white;

        public AudioClip skillFX       = null;
        public float     volum         = 1f;

        public ForceFieldSkill forceFieldSkill = null;
    }

    [Inspector(group = "Target Info")]
    [SerializeField] private PlayerCtrl m_playerCtrl = null;

    [Inspector(group = "Force Field Shader Data")]
    [SerializeField] private GameObject[] m_shaderObjArray = null;
    [SerializeField] private Material     m_shaderMaterial = null;
    [SerializeField] private float        m_shaderVisibleFadeSeconds = 0f;
    [SerializeField] private float        m_shaderBrightness         = 0.75f;

    [Inspector(group = "Skill Data")]
    [SerializeField] private SkillData[] m_skillDataArray = null;

    private AudioSource            m_audio               = null;
    private IObservable<SkillData> m_skillDataObservable = null;
    private ForceFieldSkill        m_currentSkill        = null;

    void Awake()
    {
        m_audio = GetComponent<AudioSource>();
        m_currentSkill = m_skillDataArray[0].forceFieldSkill;

        m_skillDataObservable = m_skillDataArray.ToObservable();
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .SelectMany(_ => m_skillDataObservable.Where(skill => Input.GetKeyDown(skill.acitveKeyCode)))
            .Subscribe(skill => {
                if (!skill.forceFieldSkill.IsActivated)
                {
                    m_currentSkill.OnSkillDeactivate(m_playerCtrl);
                    m_currentSkill = skill.forceFieldSkill;

                    m_currentSkill.OnSkillActivate(m_playerCtrl);
                    m_shaderMaterial.SetColor("_InnerTint", skill.innerColor);
                    m_shaderMaterial.SetColor("_OuterTint", skill.outerColor);

                    StartCoroutine("ActivateForceFieldShader");

                    m_audio.PlayOneShot(skill.skillFX, skill.volum);
                }
                else
                    m_currentSkill.IsActivated = false;
            });
    }

    void Start()
    {
        m_currentSkill = m_skillDataArray[0].forceFieldSkill;

        this.ObserveEveryValueChanged(_ => m_currentSkill.IsActivated)
            .Where(_ => this.enabled)
            .Where(isActivated => !isActivated)
            .Subscribe(_ => {
                m_currentSkill.OnSkillDeactivate(m_playerCtrl);
                SetShaderObjActive(false);
             });
    }


    void OnDisable()
    {
        if (m_currentSkill != null)
        {
            m_currentSkill.OnSkillDeactivate(m_playerCtrl);
            SetShaderObjActive(false);
        }
    }
    private void SetShaderObjActive(bool isActive)
    {
        for (int i = 0; i < m_shaderObjArray.Length; i++)
            m_shaderObjArray[i].SetActive(isActive);
    }

    private IEnumerator ActivateForceFieldShader()
    {
        float currentSeconds = 0f;

        SetShaderObjActive(true);

        while (true)
        {
            currentSeconds += Time.smoothDeltaTime;
            m_shaderMaterial.SetFloat("_Static", Mathf.Lerp(0f, m_shaderBrightness, (currentSeconds / m_shaderVisibleFadeSeconds)));

            if ((currentSeconds / m_shaderVisibleFadeSeconds) >= 1.0f)
                break;

            yield return null;
        }
    }
}
