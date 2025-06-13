using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementDisplayManager : MonoBehaviour
{
    public GameObject achievementItemPrefab;
    public Transform contentParent;

    void Start()
    {
        foreach (Achievement ach in AchievementManager.Instance.allAchievements)
        {
            GameObject item = Instantiate(achievementItemPrefab, contentParent);
            item.transform.localScale = Vector3.one;

            // ¸³ÖµÄÚÈÝ
            var icon = item.transform.Find("Icon").GetComponent<Image>();
            var title = item.transform.Find("Title").GetComponent<Text>();
            var desc = item.transform.Find("Description").GetComponent<Text>();
            var lockMask = item.transform.Find("Mask").gameObject;

            icon.sprite = ach.icon;
            title.text = ach.title;
            desc.text = ach.description;

            lockMask.SetActive(!ach.unlocked);
            icon.color = ach.unlocked ? Color.white : Color.gray;
        }
    }
}
