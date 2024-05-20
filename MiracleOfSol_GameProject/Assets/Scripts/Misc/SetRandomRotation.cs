using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRandomRotation : MonoBehaviour
{
    public Vector3 RotateInDirection = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.Euler(new Vector3(Random.Range(0, 360) * RotateInDirection[0], Random.Range(0, 360) * RotateInDirection[1], Random.Range(0, 360) * RotateInDirection[2]));
        Destroy(this);
    }
}
