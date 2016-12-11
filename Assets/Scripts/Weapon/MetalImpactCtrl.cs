using UnityEngine;
using System.Collections;

/**
 * @brief 총알 자국의 색깔 애니메이션을 담당하는 스크립트
 */
public class MetalImpactCtrl : MonoBehaviour {
    [SerializeField] private ParticleSystem[] m_metalImpactArray = null;

    public void ImpactColorChange(Color color)
    {
        for (int i = 0; i < m_metalImpactArray.Length; i++)
            m_metalImpactArray[i].startColor = color;
    }
}
