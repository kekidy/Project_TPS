using UnityEngine;
using System.Collections;
using EasyEditor;
using UnityEngine.Events;

public class ForceFieldSystem : MonoBehaviour {
    [System.Serializable]
    public class SkillData
    {
        public KeyCode    acitveKeyCode;
        public UnityEvent OnSkillActive;
    }

    [Inspector(group = "Force Field Shader Data")]
    [SerializeField] private GameObject[] m_shaderObjArray = null;
    [SerializeField] private Material     m_shaderMaterial = null;

    [Inspector(group = "Skill Data")]
    [SerializeField] private SkillData    m_speedUpSkillData = null;
	
	// Update is called once per frame
	void Update () {
	
	}
}
