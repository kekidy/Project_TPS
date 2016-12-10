using UnityEngine;
using System.Collections;

public class MetalImpactCtrl : MonoBehaviour {
    [SerializeField] private ParticleSystem[] m_metalImpactArray = null;

    public void ImpactColorChange(Color color)
    {
        for (int i = 0; i < m_metalImpactArray.Length; i++)
            m_metalImpactArray[i].startColor = color;
    }
}
