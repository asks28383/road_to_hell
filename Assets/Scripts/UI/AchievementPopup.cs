using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public static AchievementPopup Instance { get; private set; } // 添加 Singleton 引用

    public GameObject popupUI;
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;

    public float moveDistance = 50f;
    public float moveSpeed = 100f;
    public float showDuration = 3f;

    private Vector3 originalPosition;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);  // 已有实例则销毁当前对象
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        AchievementManager.Instance.onAchievementUnlocked.AddListener(ShowPopup);
        originalPosition = popupUI.transform.localPosition;
    }

    public void ShowPopup(Achievement achievement)
    {
        titleText.text = achievement.title;
        descriptionText.text = achievement.description;
        iconImage.sprite = achievement.icon;

        StopAllCoroutines();
        StartCoroutine(AnimatePopup());
    }

    IEnumerator AnimatePopup()
    {
        popupUI.SetActive(true);

        Vector3 targetDown = originalPosition + new Vector3(0, -moveDistance, 0);
        Vector3 targetUp = originalPosition;

        popupUI.transform.localPosition = targetUp;
        yield return StartCoroutine(MoveTo(targetDown));
        yield return new WaitForSeconds(showDuration);
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
