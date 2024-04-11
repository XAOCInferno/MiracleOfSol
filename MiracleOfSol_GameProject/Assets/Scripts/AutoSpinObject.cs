using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSpinObject : MonoBehaviour
{
    public bool DirectionIsRandom = false;
    public Vector3 SpinRate;

    private void Start()
    {
        if (DirectionIsRandom) { SpinRate = Vector3.Scale(SpinRate, new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1))); }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(SpinRate * Time.deltaTime);
    }
}
