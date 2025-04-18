using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    // Start is called before the first frame update
    protected override void PlayHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }

    protected override void PlayDeathAnimation()
    {
        animator.SetTrigger("Death");
    }
}
