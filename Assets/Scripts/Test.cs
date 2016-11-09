using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Test : MonoBehaviour {
    public GameObject testobj;

	// Use this for initialization
    IEnumerator Start()
    {
        yield return new WaitForSeconds(3f);

        var oper = SceneManager.LoadSceneAsync("TestScene", LoadSceneMode.Additive);
        while (!oper.isDone)
            yield return null;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
