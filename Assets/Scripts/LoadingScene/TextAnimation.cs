using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

/**
 * @brief Main Scene에서 임무가 적히는 텍스트의 애니메이션을 담당하는 스크립트
 */

public class TextAnimation : MonoBehaviour {
    [SerializeField] private Text  m_animateText        = null;
    [SerializeField] private float m_textAnimateSeconds = 0f;

    private string        m_description = string.Empty;
    private StringBuilder m_sb          = new StringBuilder();

	void Awake () {
        m_description = m_animateText.text;
        StartCoroutine("TextAnimate");
	}

    private IEnumerator TextAnimate()
    {
        for (int i = 0; i < m_description.Length; i++)
        {
            m_sb.Append(m_description[i]);
            m_animateText.text = m_sb.ToString();

            yield return new WaitForSeconds(m_textAnimateSeconds);
        }
    }
}
