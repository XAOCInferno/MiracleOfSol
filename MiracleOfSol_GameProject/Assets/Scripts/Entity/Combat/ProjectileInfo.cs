using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileInfo : MonoBehaviour
{
    public bool IHaveCollided = false;
    public GameObject CollidedObj;

    private void Start()
    {
        IHaveCollided = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Entity"))
        {
            CollidedObj = collision.gameObject;
            IHaveCollided = true;
        }
    }
}
