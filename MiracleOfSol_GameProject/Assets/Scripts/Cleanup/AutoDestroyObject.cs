using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyObject : MonoBehaviour
{
    public float TimeToDestroy = 1000;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroySelf", TimeToDestroy);
    }

    public void ResetDestroyTime(float NewTime)
    {
        CancelInvoke();

        Invoke("DestroySelf", NewTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
