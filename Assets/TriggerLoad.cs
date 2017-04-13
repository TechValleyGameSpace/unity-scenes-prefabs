using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TriggerLoad : MonoBehaviour
{
    public enum LoadMethod
    {
        SyncLoad,
        AsyncLoad,
        SyncAdditiveLoad,
        AsyncAdditiveLoad
    }

    [SerializeField]
    private LoadMethod callMethod = LoadMethod.SyncLoad;
    [SerializeField]
    private string loadSceneName = "game";
    [Header("Loading Panel")]
    [SerializeField]
    private GameObject loadingCanvas;
    [SerializeField]
    private RectTransform loadingPanel;

    private Vector2 cache;
    private bool isTriggered = false;

    private void Start()
    {
        loadingCanvas.transform.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((isTriggered == false) && (other.CompareTag("Player") == true))
        {
            isTriggered = true;
            AsyncOperation loading = null;
            switch (callMethod)
            {
                case LoadMethod.AsyncLoad:
                    Debug.Log("Calling: Application.LoadLevelAsync(\"" + loadSceneName + "\")");
                    loading = SceneManager.LoadSceneAsync(loadSceneName);
                    break;
                case LoadMethod.SyncAdditiveLoad:
                    Debug.Log("Calling: Application.LoadLevelAdditive(\"" + loadSceneName + "\")");
                    SceneManager.LoadScene(loadSceneName, LoadSceneMode.Additive);
                    break;
                case LoadMethod.AsyncAdditiveLoad:
                    Debug.Log("Calling: Application.LoadLevelAdditiveAsync(\"" + loadSceneName + "\")");
                    loading = SceneManager.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
                    break;
                case LoadMethod.SyncLoad:
                default:
                    Debug.Log("Calling: Application.LoadLevel(\"" + loadSceneName + "\")");
                    SceneManager.LoadScene(loadSceneName);
                    break;
            }
            if(loading != null)
            {
                StartCoroutine(ShowProgressBar(loading));
            }
        }
    }

    private IEnumerator ShowProgressBar(AsyncOperation loading)
    {
        loadingCanvas.SetActive(true);
        cache = loadingPanel.anchorMax;
        cache.x = 0;
        loadingPanel.anchorMax = cache;
        while (loading.isDone == false)
        {
            yield return null;
            cache.x = loading.progress;
            loadingPanel.anchorMax = cache;
        }
        loadingCanvas.SetActive(false);
    }
}
