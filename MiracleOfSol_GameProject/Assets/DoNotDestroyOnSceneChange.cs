using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroyOnSceneChange : MonoBehaviour
{
    void Awake()
    { 
        DontDestroyOnLoad(transform.gameObject); 
    }
}
