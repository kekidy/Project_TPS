using UnityEngine;
using TMPro;
using UniRx;
using UniRx.Triggers;
using System.Collections;

/**
 * @brief 텍스트 메쉬에 다가가면 텍스트가 점점 사라지고 멀어지면 점점 나타나는 애니메이션 제어 스크립트
 */

public class TextHideCtrl : MonoBehaviour {
    [SerializeField] private Transform m_targetTransform   = null;
    [SerializeField] private Transform m_centerTransform   = null;
    [SerializeField] private float     m_hideStartDistance = 0f;

    private TextMeshPro m_textMeshPro = null;
    private Color       m_originColor = Color.white;
	private Vector2     m_originPosXZ = Vector2.zero;
      
	void Awake () {
        m_textMeshPro = GetComponent<TextMeshPro>();
        Vector3 pos   = m_centerTransform.position;
        m_originPosXZ = new Vector2(pos.x, pos.z);
        m_originColor = m_textMeshPro.color;   

        this.LateUpdateAsObservable()
            .Select(_ => {
                Vector3 targetPos = m_targetTransform.position;
                Vector2 targetPosXZ = new Vector2(targetPos.x, targetPos.z);
                float dist = Vector2.Distance(targetPosXZ, m_originPosXZ);
                return dist;
            })
            .Where(dist => dist < m_hideStartDistance)
            .Subscribe(dist => {
                float alpha = Mathf.Lerp(0f, 1f, ((dist - 1.5f) / m_hideStartDistance));
                m_textMeshPro.color = new Color(m_originColor.r, m_originColor.g, m_originColor.b, alpha);
            });

	}
}
