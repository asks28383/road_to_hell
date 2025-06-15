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

    // ��֤��ʼ��ֵΪ 1����ȫ����ʾ��
    dissolveMat.SetFloat("_DissolveThreshold", 1f);
    yield return null;

    // 1 �� 0�����ԣ����룬��ס�ɻ��棩
    yield return dissolveMat
        .DOFloat(0f, "_DissolveThreshold", 1.5f)
        .SetEase(Ease.InOutSine)
        .WaitForCompletion();

    // �����³���
    SceneManager.LoadScene(sceneName);
    yield return null; // ��һ֡��ִ�е���

    // ȷ���л����� 0���ڵ���
    dissolveMat.SetFloat("_DissolveThreshold", 0f);

    // 0 �� 1����������������ʾ�»��棩
    yield return dissolveMat
        .DOFloat(1f, "_DissolveThreshold", 1.5f)
        .SetEase(Ease.InOutSine)
        .WaitForCompletion();

    // ��ȫ��ȥ��ر� RawImage
    dissolveImage.enabled = false;
}


}
