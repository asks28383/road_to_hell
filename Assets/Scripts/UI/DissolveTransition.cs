using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class DissolveTransition : MonoBehaviour
{
    public static DissolveTransition Instance { get; private set; }

    public RawImage dissolveImage;
    private Material dissolveMat;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 若已有实例，则销毁当前重复对象
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 确保每个实例有独立材质
        dissolveMat = Instantiate(dissolveImage.material);
        dissolveImage.material = dissolveMat;
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(DissolveAndLoad(sceneName));
    }

    private IEnumerator DissolveAndLoad(string sceneName)
    {
        dissolveImage.enabled = true;

        // 淡入：盖住旧画面
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
        yield return dissolveMat
            .DOFloat(0f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        SceneManager.LoadScene(sceneName);

        yield return null; // 等下一帧新场景加载完成
        yield return null;

        // 淡出：展示新画面
        dissolveMat.SetFloat("_DissolveThreshold", 0f);
        yield return dissolveMat
            .DOFloat(1f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        dissolveImage.enabled = false;
    }
}
