using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class perfecttime: MonoBehaviour
{
    public Animator ani;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(XuLiBar.instance.isfull)
        {
            ani.SetBool("isfull", true);
        }
        else if(!XuLiBar.instance.isfull)
        {
            ani.SetBool("isfull", false);
        }
    }
}
