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
        }
    }

    private void SaveAchievement(Achievement ach)
    {
        PlayerPrefs.SetInt(ach.achievementID, 1);
        PlayerPrefs.Save();
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
}
