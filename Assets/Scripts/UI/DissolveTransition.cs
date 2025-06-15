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

        // 淡入（遮挡当前场景）
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
        yield return dissolveMat
            .DOFloat(0f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        // 同步加载场景（确保数据一致性）
        SceneManager.LoadScene(sceneName);

        // 等待场景完全加载
        yield return null;
        yield return null;

        // 淡出（显示新场景）
        dissolveMat.SetFloat("_DissolveThreshold", 0f);
        yield return dissolveMat
            .DOFloat(1f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        dissolveImage.enabled = false;
    }


}
