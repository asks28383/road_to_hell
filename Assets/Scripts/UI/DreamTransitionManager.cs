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

        // 确保组件存在
        if (dissolveTransition == null)
        {
            dissolveTransition = gameObject.AddComponent<DissolveTransition>();
            if (dissolveImage != null)
            {
                dissolveTransition.dissolveImage = dissolveImage;
            }
        }
    }

    public void TransitionToScene(string sceneName)
    {
        if (dissolveTransition != null)
            dissolveTransition.TransitionToScene(sceneName);
        else
            Debug.LogWarning("DissolveTransition 未设置！");
    }
}
