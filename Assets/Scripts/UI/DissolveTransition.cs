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
        dissolveMat = Instantiate(dissolveImage.material); // ÿ��ʵ����������
        dissolveImage.material = dissolveMat;
        // Awake ������
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

        // ���루�ڵ���ǰ������
        dissolveMat.SetFloat("_DissolveThreshold", 1f);
        yield return dissolveMat
            .DOFloat(0f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        // ͬ�����س�����ȷ������һ���ԣ�
        SceneManager.LoadScene(sceneName);

        // �ȴ�������ȫ����
        yield return null;
        yield return null;

        // ��������ʾ�³�����
        dissolveMat.SetFloat("_DissolveThreshold", 0f);
        yield return dissolveMat
            .DOFloat(1f, "_DissolveThreshold", 1.5f)
            .SetEase(Ease.InOutSine)
            .WaitForCompletion();

        dissolveImage.enabled = false;
    }


}
