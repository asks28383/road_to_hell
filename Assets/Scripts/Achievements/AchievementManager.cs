using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    public List<Achievement> allAchievements;

    public UnityEvent<Achievement> onAchievementUnlocked;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadAchievements();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadAchievements()
    {
        foreach (Achievement ach in allAchievements)
        {
            ach.unlocked = PlayerPrefs.GetInt(ach.achievementID, 0) == 1;
            //Debug.Log($"Achievement {ach.title} loaded: {(ach.unlocked ? "Unlocked" : "Locked")}");
            //删除保存的成就记录
            //这个只是测试用，日后删除
            DeleteAchievement(ach);
        }
    }

    private void SaveAchievement(Achievement ach)
    {
        PlayerPrefs.SetInt(ach.achievementID, 1);
        PlayerPrefs.Save();
    }

    private void DeleteAchievement(Achievement ach)
    {
        PlayerPrefs.DeleteKey(ach.achievementID);
        ach.unlocked = false; // Reset the achievement state
        //Debug.Log($"Achievement {ach.title} deleted");
    }

    public void UnlockAchievement(string id)
    {
        Achievement ach = allAchievements.Find(a => a.achievementID == id);
        if (ach != null && !ach.unlocked)
        {
            ach.unlocked = true;
            SaveAchievement(ach);
            onAchievementUnlocked?.Invoke(ach);
        }
    }

    private void OnEnable()
    {
        AchievementEvents.OnAchievementTriggered += UnlockAchievement;
    }

    private void OnDisable()
    {
        AchievementEvents.OnAchievementTriggered -= UnlockAchievement;
    }
}
