using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class changeScene : MonoBehaviour
{
    //public DissolveTransition transition;

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
                LoadSceneWithTransition(sceneButton.sceneName);
                //DreamTransitionManager.Instance.TransitionToScene("DreamScene");

                //transition.TransitionToScene(sceneButton.sceneName);
                if (sceneButton.sceneName == "LevelSelectScene")
                {
                    AchievementEvents.OnAchievementTriggered?.Invoke("FirstEntertheGame");
                }
            });
        }
    }

    void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    void LoadSceneWithTransition(string sceneName)
    {
        FadeController.Instance.FadeToScene(sceneName);
    }
}