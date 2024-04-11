using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButtonVisibility : MonoBehaviour
{
    public bool IsAlt = false;

    private UnityEngine.UI.Button Btn;
    private UnityEngine.UI.Image Img;

    // Start is called before the first frame update
    void Start()
    {
        Btn = gameObject.GetComponent<UnityEngine.UI.Button>();
        Img = gameObject.GetComponent<UnityEngine.UI.Image>();
        InvokeRepeating(nameof(LessOftenUpdate), 0, 0.1f + Random.Range(-5, 5) / 100);
    }

    private void LessOftenUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (IsAlt) { Btn.enabled = true; Img.enabled = true; }
            else { Btn.enabled = false; Img.enabled = false; }
        }
        else
        {
            if (IsAlt) { Btn.enabled = false; Img.enabled = false; }
            else { Btn.enabled = true; Img.enabled = true; }
        }
    }
}
