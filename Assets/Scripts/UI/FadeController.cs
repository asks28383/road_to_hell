using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance;
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
        fadeImage.color = new Color(0, 0, 0, 1f); // 全黑
        fadeImage.DOFade(0f, 1f); // 一秒淡入场景

    }

    public void FadeToScene(string sceneName)
    {
        fadeImage.raycastTarget = true;
        fadeImage.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            SceneManager.LoadScene(sceneName);
            // 场景加载完后再淡出
            SceneManager.sceneLoaded += OnSceneLoaded;
        });
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        fadeImage.DOFade(0f, fadeDuration).OnComplete(() =>
        {
            fadeImage.raycastTarget = false;
        });
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
