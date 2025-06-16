using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class ButtonSoundHelper : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable)
            UISoundManager.Instance.PlayButtonHover();
    }

    public void PlayClickSound()
    {
        UISoundManager.Instance.PlayButtonClick();
    }
}