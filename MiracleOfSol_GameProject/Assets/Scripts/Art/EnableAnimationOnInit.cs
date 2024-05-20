using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAnimationOnInit : MonoBehaviour
{
    public Animation TargetAnim;

    // Start is called before the first frame update
    void Start()
    {
        TargetAnim.Play();
        Destroy(gameObject);
    }
}
