using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public GameObject popupUI;
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;

    public float moveDistance = 50f;      // �����ƶ��ľ���
    public float moveSpeed = 100f;        // �ƶ��ٶȣ�����/�룩
    public float showDuration = 3f;       // ͣ��ʱ��

    private Vector3 originalPosition;

    private void Start()
    {
        AchievementManager.Instance.onAchievementUnlocked.AddListener(ShowPopup);
        originalPosition = popupUI.transform.localPosition;
        DontDestroyOnLoad(this.gameObject);

    }

    public void ShowPopup(Achievement achievement)
    {
        titleText.text = achievement.title;
        descriptionText.text = achievement.description;
        iconImage.sprite = achievement.icon;

        StopAllCoroutines();  // ȷ�������ж����������
        StartCoroutine(AnimatePopup());
    }

    IEnumerator AnimatePopup()
    {
        popupUI.SetActive(true);

        Vector3 targetDown = originalPosition + new Vector3(0, -moveDistance, 0);
        Vector3 targetUp = originalPosition;

        // ��ʼ�ص�ԭʼλ�ã����ⷴ���ƶ�����λ����
        popupUI.transform.localPosition = targetUp;

        // �����ƶ�
        yield return StartCoroutine(MoveTo(targetDown));

        // ͣ��ʱ��
        yield return new WaitForSeconds(showDuration);

        // ���ϻص�ԭλ
        yield return StartCoroutine(MoveTo(targetUp));

        popupUI.SetActive(false);
    }

    IEnumerator MoveTo(Vector3 targetPos)
    {
        while (Vector3.Distance(popupUI.transform.localPosition, targetPos) > 0.1f)
        {
            popupUI.transform.localPosition = Vector3.MoveTowards(
                popupUI.transform.localPosition,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        popupUI.transform.localPosition = targetPos;
    }
}
