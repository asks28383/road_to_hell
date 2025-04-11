using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitApplication()
    {
        // 在打包后的游戏中退出
        Application.Quit();

        // 在编辑器中停止播放模式
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}