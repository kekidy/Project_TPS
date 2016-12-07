using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;
using EasyEditor;

[RequireComponent(typeof(AudioSource))]
public class ElementalBulletSystem : MonoBehaviour {
    [System.Serializable]
    private class BulletData
    {
        public string    skillName      = string.Empty;
        public KeyCode   acitveKeyCode  = KeyCode.None;

        public AudioClip bulletFX       = null;
        public float     volum          = 1f;

        public ElementalBullet elementalBullet = null;
    }

    public static ElementalBulletSystem Instance { get; private set; }

    [SerializeField] private BulletData[] m_bulletDataArray = null;

    private AudioSource             m_audio               = null;
    private IObservable<BulletData> m_skillDataObservable = null;
    private ElementalBullet         m_currentElemental    = null;
    
    public ElementalBullet CurrentElementalBullet { get { return m_currentElemental; } }

    void Awake()
    {
        Instance = this;
        m_audio  = GetComponent<AudioSource>();

        m_skillDataObservable = m_bulletDataArray.ToObservable();
        this.UpdateAsObservable()
            .Where(_ => this.enabled)
            .SelectMany(_ => m_skillDataObservable.Where(skill => Input.GetKeyDown(skill.acitveKeyCode)))
            .Subscribe(bullet =>
            {
                if (!bullet.elementalBullet.IsActivated)
                {
                    if (m_currentElemental != null)
                        m_currentElemental.IsActivated = false;

                    m_currentElemental = bullet.elementalBullet;
                    m_currentElemental.IsActivated = true;

                    m_audio.PlayOneShot(bullet.bulletFX, bullet.volum);
                }
                else
                {
                    m_currentElemental.IsActivated = false;
                    m_currentElemental = null;
                }
            });
    }
}
