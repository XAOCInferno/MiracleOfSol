using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimateUIElement : MonoBehaviour
{
    public Vector3 MoveToLocation = new Vector3(0, 2);
    public float MoveVel = 1;

    private TextMeshProUGUI TextItem;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, transform.position + MoveToLocation, MoveVel * Time.deltaTime);
    }
}
