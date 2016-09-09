using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TaskSpriteType { NORMAL = 0, SUCCESS }

public class TaskCtrl : MonoBehaviour {
    [SerializeField] private Sprite m_normalSprite;
    [SerializeField] private Sprite m_successSprtie;
    [SerializeField] private TaskSpriteType m_currentSpriteType;

    private GameObject m_myGameObject;
    private Image m_myImage;
    private Text  m_myText;

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

    public bool GameObjectActive
    {
        get { return m_myGameObject.activeSelf; }
        set { m_myGameObject.SetActive(value); }
    }

	void Awake () {
        m_myGameObject = gameObject;
        m_myImage      = GetComponent<Image>();
        m_myText       = GetComponentInChildren<Text>();
        TaskSpriteType = m_currentSpriteType;
	}
}
