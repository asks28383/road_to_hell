using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public GameObject popupUI;
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;

    public float moveDistance = 50f;      // 向下移动的距离
    public float moveSpeed = 100f;        // 移动速度（像素/秒）
    public float showDuration = 3f;       // 停留时间

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

        StopAllCoroutines();  // 确保不会有多个动画叠加
        StartCoroutine(AnimatePopup());
    }

    IEnumerator AnimatePopup()
    {
        popupUI.SetActive(true);

        Vector3 targetDown = originalPosition + new Vector3(0, -moveDistance, 0);
        Vector3 targetUp = originalPosition;

        // 初始回到原始位置（避免反复移动产生位移误差）
        popupUI.transform.localPosition = targetUp;

        // 向下移动
        yield return StartCoroutine(MoveTo(targetDown));

        // 停留时间
        yield return new WaitForSeconds(showDuration);

        // 向上回到原位
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
