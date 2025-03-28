using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObject SelectImage; // 需要显示的子图片
    private GameObject LockImage;
    private GameObject DeadImage;

    void Start()
    {
        LockImage = transform.GetChild(0).gameObject;
        DeadImage = transform.GetChild(1).gameObject;
        SelectImage = transform.GetChild(2).gameObject; 
        if (SelectImage != null)
        {
            LockImage.SetActive(false);
            DeadImage.SetActive(false);
            SelectImage.SetActive(false);

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectImage != null)
        {
            SelectImage.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectImage != null)
        {
            SelectImage.SetActive(false);
        }
    }
}

