using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OnActivateChangeVFX : MonoBehaviour
{
    public VisualEffect[] VFXToChange;
    public Gradient NewColour;

    // Start is called before the first frame update
    void Start()
    {
        if(NewColour != new Gradient())
        {
            foreach (VisualEffect tmpVFX in VFXToChange)
            {
                tmpVFX.SetGradient("ColourOverTime", NewColour);
            }
        }
        Destroy(gameObject);
    }
}
