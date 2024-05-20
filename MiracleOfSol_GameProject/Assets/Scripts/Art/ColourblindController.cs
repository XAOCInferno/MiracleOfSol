using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ColourblindController : MonoBehaviour
{
    public VolumeProfile[] ColourblindVolumes;
    private Volume CurrentVolume;

    // Start is called before the first frame update
    void Start()
    {
        CurrentVolume = gameObject.GetComponent<Volume>();
        UpdateColourblindMode();
    }

    public void UpdateColourblindMode()
    {
        CurrentVolume.profile = ColourblindVolumes[PlayerPrefs.GetInt("ColourblindMode", 0)];
    }
}
