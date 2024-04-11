using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosAsParentPos : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.parent != null)
        {
            transform.position = transform.parent.position;
            transform.rotation = new Quaternion();
        }
    }
}
