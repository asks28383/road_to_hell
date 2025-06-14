using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;
[TaskCategory("Custom")]
public class BulletAttack : Action
{
    // Start is called before the first frame update
    public GameObject boss;
    public override void OnStart()
    {
        boss.GetComponent<BulletConfig>().enabled = true;
    }

    // Update is called once per frame
    public override TaskStatus OnUpdate()
    {
        // Return success once we've disabled the component
        return TaskStatus.Running;
    }
}
