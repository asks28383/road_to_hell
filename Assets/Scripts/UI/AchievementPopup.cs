using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AchievementPopup : MonoBehaviour
{
    public GameObject popupUI;
    public Text titleText;
    public Text descriptionText;
    public Image iconImage;

    private void Start()
    {
        AchievementManager.Instance.onAchievementUnlocked.AddListener(ShowPopup);
    }

    public void ShowPopup(Achievement achievement)
    {
        titleText.text = achievement.title;
        descriptionText.text = achievement.description;
        iconImage.sprite = achievement.icon;
        StartCoroutine(ShowForSeconds(3f));
    }

    IEnumerator ShowForSeconds(float seconds)
    {
        popupUI.SetActive(true);
        yield return new WaitForSeconds(seconds);
        popupUI.SetActive(false);
    }
}
