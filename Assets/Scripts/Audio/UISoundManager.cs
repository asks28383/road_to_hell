using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    private static UISoundManager _instance;
    public static UISoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 动态加载 Resources 中的预制体
                GameObject prefab = Resources.Load<GameObject>("UISoundManager");
                if (prefab == null)
                {
                    Debug.LogError("UISoundManager prefab not found in Resources!");
                    return null;
                }

                GameObject go = Instantiate(prefab);
                _instance = go.GetComponent<UISoundManager>();

                if (_instance == null)
                    Debug.LogError("UISoundManager script missing on prefab!");
            }

            return _instance;
        }
    }

    [Header("Button Sounds")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip buttonSelectSound;

    private AudioSource audioSource;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayButtonClick()
    {
        if (audioSource && buttonClickSound)
            audioSource.PlayOneShot(buttonClickSound);
    }

    public void PlayButtonHover()
    {
        if (audioSource && buttonHoverSound)
            audioSource.PlayOneShot(buttonHoverSound);
    }
}
