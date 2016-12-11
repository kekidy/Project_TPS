using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TaskSpriteType { NORMAL = 0, SUCCESS }

/**
 * @brief 해야할 임무 목록을 보여주는 Task UI를 제어하는 컨트롤러 스크립트(개발 진행중)
 */

public class TaskCtrl : MonoBehaviour {
    [SerializeField] private Sprite m_normalSprite;
    [SerializeField] private Sprite m_successSprtie;
    
    
    private Image m_myImage;
    private Text  m_myText;
    private TaskSpriteType m_currentSpriteType;

    public TaskSpriteType TaskSpriteType
    {
        get { return m_currentSpriteType; }
        set
        {
            m_currentSpriteType = value;
            m_myImage.sprite = (m_currentSpriteType == TaskSpriteType.NORMAL) ? m_normalSprite : m_successSprtie;
        }
    }

    public string Message
    {
        get { return m_myText.text; }
        set { m_myText.text = value; }
    }

	void Awake () {
        m_myImage      = GetComponent<Image>();
        m_myText       = GetComponentInChildren<Text>();
	}
}
