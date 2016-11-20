using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour {
    [SerializeField] private string m_nextSceneName = string.Empty;
    [SerializeField] private Image  m_progressImage = null;
    [SerializeField] private Text   m_loadtext;

    private AsyncOperation m_asyncOper = null;

    IEnumerator Start () {
        m_progressImage.fillAmount = 0f;
        yield return new WaitForSeconds(2.5f);
        StartCoroutine("AsyncSceneLoad");
	}

    private IEnumerator AsyncSceneLoad()
    {
        m_asyncOper = SceneManager.LoadSceneAsync(m_nextSceneName);
        m_asyncOper.allowSceneActivation = false;

        while (m_asyncOper.progress < 0.9f)
        {
            m_progressImage.fillAmount = m_asyncOper.progress;
            yield return null;
        }

        m_progressImage.fillAmount = 1.0f;
        m_loadtext.text = "Spacebar를 누르면 임무를 시작됩니다";

        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            yield return null;
        }

        m_asyncOper.allowSceneActivation = true;
    }
}
