using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitApplication()
    {
        // �ڴ�������Ϸ���˳�
        Application.Quit();

        // �ڱ༭����ֹͣ����ģʽ
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}