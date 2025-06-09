using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject achievementItemPrefab;

    private void Start()
    {
        foreach (Achievement ach in AchievementManager.Instance.allAchievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentParent);
            item.transform.Find("Title").GetComponent<Text>().text = ach.title;
            item.transform.Find("Description").GetComponent<Text>().text = ach.description;
            item.transform.Find("Icon").GetComponent<Image>().sprite = ach.icon;
            item.transform.Find("LockedOverlay").gameObject.SetActive(!ach.unlocked);
        }
    }
}
