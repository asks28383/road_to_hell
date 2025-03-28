using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterButtonController : MonoBehaviour
{
    [SerializeField] private Button[] Buttons;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Buttons.Length; i++)
        {
            int index = i; // 捕获当前索引，避免闭包问题
            Buttons[i].onClick.AddListener(() => OnButtonClicked(index));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnButtonClicked(int index)
    {
        Debug.Log("Button " + index + " Clicked");
        UIController.instance.UpdatePlayer(index);
    }
}
