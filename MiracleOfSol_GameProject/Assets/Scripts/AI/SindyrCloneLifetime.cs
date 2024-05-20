using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SindyrCloneLifetime : MonoBehaviour
{
    private float Lifetime = 20;
    
    private void Start()
    {
        Invoke(nameof(AutoDestroySelf), Lifetime + Random.Range(-5,5));
    }

    public void AutoDestroySelf()
    {
        gameObject.GetComponent<Health>().ForceKillSelf();
    }
}
