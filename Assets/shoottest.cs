using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shoottest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //是否按下鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse button clicked");
            Debug.Log("Achievement triggered: firstshoot");
            AchievementEvents.OnAchievementTriggered?.Invoke("firstshoot");
        }
    }
}
