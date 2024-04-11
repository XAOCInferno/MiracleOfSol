using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnEnable_UpdateFont : MonoBehaviour
{
    private MasterSettingsController MSC;
    private TMPro.TextMeshProUGUI self_text;

    // Start is called before the first frame update
    void Start()
    {
        InitLogic();
    }

    private void OnEnable()
    {
        InitLogic();
    }

    private void UpdateFont()
    {
        if (self_text != MSC.CurrentFont)
        {
            self_text.font = MSC.CurrentFont;
        }
    }

    private void InitLogic()
    {
        CancelInvoke();
        if (MSC == null) { GameObject.FindGameObjectWithTag("SettingsManager").TryGetComponent(out MSC); }
        if (self_text == null) { gameObject.TryGetComponent(out self_text); }

        InvokeRepeating(nameof(UpdateFont), 0.1f + Random.Range(0.1f, 0.3f), 0.5f + Random.Range(-0.25f, 0.25f));
    }
}
