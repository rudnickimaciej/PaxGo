using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ScenesManager : MonoBehaviour
{

    [Header("UI")]
    public GameObject LoadingUI;
    AsyncOperation async;
    bool loading;
    public void LoadScene(int sceneIndex)
    {
        loading = true;
        StartCoroutine(Load(sceneIndex));
    }
    public IEnumerator Load(int sceneIndex)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
        async.allowSceneActivation = false;
        LoadingUI.SetActive(true);

        while (loading)
        {
            if (async.progress > 0.89)
            {
                loading = false;
                async.allowSceneActivation = true;
            }
        }
        LoadingUI.SetActive(false);
        yield return null;
    }

}