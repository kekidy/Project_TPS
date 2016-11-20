using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct ScriptData
{
    public string    name;
    public string    script;
    public float     duration;
    public bool      isEnd;
    public AudioClip audioClip;
}

[CreateAssetMenu(fileName = "ScriptDataBase", menuName = "ScriptDataBase")]
public class ScriptDataBase : ScriptableObject {
    [SerializeField] private List<ScriptData> m_scriptDataList = null;

    public ScriptData this[int index] { get { return m_scriptDataList[index]; } }
}
