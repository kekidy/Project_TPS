using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public abstract class ForceFieldSkill {
    [SerializeField] private UnityEvent onSkillActive;
    [SerializeField] private UnityEvent onSkillDeactive;
}
