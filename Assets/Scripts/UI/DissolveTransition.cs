using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class DissolveTransition : MonoBehaviour
{
    public RawImage dissolveImage;
    private Material dissolveMat;

    private void Awake()
    {
        dissolveMat = Instantiate(dissolveImage.material); // 每个实例独立控制
        dissolveImage.material = dissolveMat;
        // Awake 中设置
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
        DontDestroyOnLoad(gameObject);
    }

    public void TransitionToScene(string sceneName)
    {
        StartCoroutine(DissolveAndLoad(sceneName));
    }

    private IEnumerator DissolveAndLoad(string sceneName)
{
    dissolveImage.enabled = true;

    // 保证起始阈值为 1（完全不显示）
    dissolveMat.SetFloat("_DissolveThreshold", 1f);
    yield return null;

    // 1 → 0：渐显（淡入，盖住旧画面）
    yield return dissolveMat
        .DOFloat(0f, "_DissolveThreshold", 1.5f)
        .SetEase(Ease.InOutSine)
        .WaitForCompletion();

    // 加载新场景
    SceneManager.LoadScene(sceneName);
    yield return null; // 等一帧再执行淡出

    // 确保切换后还是 0（遮挡）
    dissolveMat.SetFloat("_DissolveThreshold", 0f);

    // 0 → 1：渐隐（淡出，显示新画面）
    yield return dissolveMat
        .DOFloat(1f, "_DissolveThreshold", 1.5f)
        .SetEase(Ease.InOutSine)
        .WaitForCompletion();

    // 完全隐去后关闭 RawImage
    dissolveImage.enabled = false;
}


}
