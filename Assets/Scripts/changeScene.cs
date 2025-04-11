using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class MultiSceneLoader : MonoBehaviour
{
    [System.Serializable]
    public class SceneButton
    {
        public Button button;
        public string sceneName;
    }

    [SerializeField] private List<SceneButton> sceneButtons = new List<SceneButton>();

    void Start()
    {
        foreach (var sceneButton in sceneButtons)
        {
            sceneButton.button.onClick.AddListener(() =>
            {
                LoadScene(sceneButton.sceneName);
            });
        }
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}