using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private GameObject lockImage;    // 🔒 未解锁时显示
    [SerializeField] private GameObject deadImage;   // 💀 已通关时显示
    [SerializeField] private GameObject selectImage;  // 🌟 悬停效果

    [Header("Level Settings")]
    [SerializeField] private string levelKey;        // 例如 "Level1_Completed"
    [SerializeField] private string prevLevelKey;    // 前一关的Key（Level1留空）

    private Button button;
    private bool isUnlocked;
    private bool isCompleted;

    void Start()
    {
        button = GetComponent<Button>();
        UpdateLevelStatus();
    }

    private void UpdateLevelStatus()
    {
        // 检查是否通关
        isCompleted = PlayerPrefs.GetInt(levelKey, 0) == 1;

        // 检查是否解锁（Level1默认解锁，其他关需前一关通关）
        isUnlocked = string.IsNullOrEmpty(prevLevelKey) ||
                    PlayerPrefs.GetInt(prevLevelKey, 0) == 1;

        // 更新UI
        lockImage.SetActive(!isUnlocked);
        deadImage.SetActive(isCompleted);
        selectImage.SetActive(false);

        // 锁定关卡不可点击
        button.interactable = isUnlocked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isUnlocked && !isCompleted)
            selectImage.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selectImage.SetActive(false);
    }

    // 按钮点击事件（需绑定到Button组件的OnClick）
    public void OnButtonClick()
    {
        if (isUnlocked)
            SceneManager.LoadScene(levelKey.Replace("_Completed", ""));
    }
}