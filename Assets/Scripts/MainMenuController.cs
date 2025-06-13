using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class start : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        PlayerPrefs.DeleteKey("PlayerHealth");
        PlayerPrefs.DeleteKey("BossHealth");
        PlayerPrefs.DeleteKey("DreamEntryCount");
        PlayerPrefs.DeleteKey("HasTriggeredFirstDream");
        PlayerPrefs.DeleteKey("HasTriggeredFirstDream");
        PlayerPrefs.DeleteKey("HasTriggeredSecondDream");
        SceneManager.LoadScene("zx");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
