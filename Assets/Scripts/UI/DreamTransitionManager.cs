using UnityEngine;
using UnityEngine.UI;

public class DreamTransitionManager : MonoBehaviour
{
    public static DreamTransitionManager Instance;

    [Header("Dissolve���")]
    public DissolveTransition dissolveTransition;

    [Header("UI ����")]
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

        // ȷ���������
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
            Debug.LogWarning("DissolveTransition δ���ã�");
    }
}
