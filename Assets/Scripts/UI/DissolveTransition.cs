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
            Destroy(gameObject); // ������ʵ���������ٵ�ǰ�ظ�����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ȷ��ÿ��ʵ���ж�������
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

        // ���룺��ס�ɻ���
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
        yield return dissolveMat
            .DOFloat(0f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        SceneManager.LoadScene(sceneName);

        yield return null; // ����һ֡�³����������
        yield return null;

        // ������չʾ�»���
        dissolveMat.SetFloat("_DissolveThreshold", 0f);
        yield return dissolveMat
            .DOFloat(1f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        dissolveImage.enabled = false;
    }
}
