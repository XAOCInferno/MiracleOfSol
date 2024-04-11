using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathLayer : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Entity")
        {
            Destroy(collision.gameObject);
        }
    }
}
