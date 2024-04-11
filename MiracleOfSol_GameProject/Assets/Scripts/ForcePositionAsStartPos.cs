using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePositionAsStartPos : MonoBehaviour
{
    private Vector3 StartPos;
    private Quaternion StartRotation;
    
    // Start is called before the first frame update
    void Start()
    {
        StartPos = transform.localPosition;
        StartRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = StartPos;
        transform.localRotation = StartRotation;
    }
}
