using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [HideInInspector]
    public float hitPoints;
    public override IEnumerator DamageCharacter(int damage, float interval)
    {
        while (true)
        {
            StartCoroutine(FlickerCharacter());
            hitPoints = hitPoints - damage;
            if (hitPoints <= float.Epsilon)
            {
                KillCharacter();
                break;
            }
            if (interval > float.Epsilon)
            {
                yield return new WaitForSeconds(interval);
            }
            else
            {
                break;
            }
        }
    }

    public override void ResetCharacter()
    {
        hitPoints = startingHitPoints;
    }

    // Start is called before the first frame update

    void Start()
    {
        ResetCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
