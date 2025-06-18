
 using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PressableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private RectTransform childTransform;    // ���ֵ�RectTransform
    public float pressDepth = 5f;          // ����ʱ��ƫ����
    private GameObject child;

    private Vector3 originalPos;

    void Start()
    {
        child = transform.GetChild(0).gameObject;
        childTransform = child.GetComponent<RectTransform>();
        originalPos = childTransform.localPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        childTransform.localPosition = originalPos + new Vector3(0, -pressDepth, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        childTransform.localPosition = originalPos;
    }
}

