using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealthController : MonoBehaviour
{
    public static CharacterHealthController instance;
    public int MaxHealthValue;
    public int preHealthValue;
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
    /// <summary>
    //value is the change value of the chara;minis zero is also ok;
    /// </summary>
    /// <param name="value"></param>
    private void changeHP(int value)
    {
        preHealthValue += value;
        HealthBar.Instance.UpdateHealth(preHealthValue);//update Health UI;
    }

}
