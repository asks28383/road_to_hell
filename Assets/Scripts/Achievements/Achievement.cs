using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievement", menuName = "Achievements/Achievement")]
[System.Serializable]
public class Achievement : ScriptableObject
{
    public string achievementID;
    public string title;
    public string description;
    public Sprite icon;
    //public Sprite icon;
    public bool unlocked; // 是否解锁成就
}

