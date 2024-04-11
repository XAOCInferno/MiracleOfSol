using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMoveDown : MonoBehaviour
{
    public float vel = 5;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position[0], transform.position[1] - (vel * Time.deltaTime), transform.position[2]);
    }
}
