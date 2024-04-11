using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWithSelf : MonoBehaviour
{
    private Collider selfCol;

    private void Start()
    {
        gameObject.TryGetComponent(out selfCol);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "IgnoreCollision")
        {
            Physics.IgnoreCollision(collision.collider, selfCol);
        }
    }
}
