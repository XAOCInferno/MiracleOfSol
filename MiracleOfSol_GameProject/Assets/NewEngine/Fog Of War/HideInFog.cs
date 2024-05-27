using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideInFog : MonoBehaviour
{
    [SerializeField] private Canvas[] UIRenderers;
    [SerializeField] private Renderer[] Renderers;

    private void OnEnable()
    {
        Actions.OnRegisterHideInFogEntity.InvokeAction(new(transform, this));
    }

    private void OnDisable()
    {
        Actions.OnDeRegisterHideInFogEntity.InvokeAction(new(transform, this));
    }

    public void SetArtView(bool status)
    {
        if (status)
        {
            EnableOrDisableArt(true);
        }
        else
        {
            EnableOrDisableArt(false);
        }
    }

    private void EnableOrDisableArt(bool status)
    {

        for (int i = 0; i < Renderers.Length; i++)
        {
            {
                try
                {

                    Renderers[i].enabled = status;

                }
                catch
                {

                    Dbg.Log("Cannot disable Renderer", eLogType.Error, eLogVerbosity.Simple);

                    if (Renderers[i] == null)
                    {

                        List<Renderer> tmpRenderes = new List<Renderer>();
                        tmpRenderes = Renderers.ToList();
                        tmpRenderes.RemoveAt(i);
                        Renderers = tmpRenderes.ToArray();

                    }

                }
            }
        }

        for (int i = 0; i < UIRenderers.Length; i++)
        {
            {
                try
                {

                    UIRenderers[i].enabled = status;

                }
                catch
                {

                    Dbg.Log("Cannot disable UI Renderer", eLogType.Error, eLogVerbosity.Simple);

                    if (UIRenderers[i] == null)
                    {

                        List<Canvas> tmpRenderes = new List<Canvas>();
                        tmpRenderes = UIRenderers.ToList();
                        tmpRenderes.RemoveAt(i);
                        UIRenderers = tmpRenderes.ToArray();

                    }

                }
            }
        }
    }
}
