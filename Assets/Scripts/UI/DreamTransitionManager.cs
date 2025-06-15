using UnityEngine;
using UnityEngine.UI;

public class DreamTransitionManager : MonoBehaviour
{
    public static DreamTransitionManager Instance;

    [Header("Dissolve组件")]
    public DissolveTransition dissolveTransition;

    [Header("UI 遮罩")]
    public RawImage dissolveImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 自动绑定 DissolveTransition 和 RawImage
        if (dissolveTransition == null)
            dissolveTransition = GetComponentInChildren<DissolveTransition>();

        if (dissolveImage == null && dissolveTransition != null)
            dissolveImage = dissolveTransition.dissolveImage;
    }

    public void TransitionToScene(string sceneName)
    {
        if (dissolveTransition != null)
            dissolveTransition.TransitionToScene(sceneName);
        else
            Debug.LogWarning("DissolveTransition 未设置！");
    }
}
