using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image stage;
    public Sprite character1, character2, character3;
    public static UIController instance;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdatePlayer(int index)
    {
        switch(index)
        {
            case 0:
                stage.sprite = character1;
                break;
            case 1:
                stage.sprite = character2;
                break;
            case 2:
                stage.sprite = character3;
                break;
        }
    }
}
