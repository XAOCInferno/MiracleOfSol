using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBossMusic : MonoBehaviour
{
    public bool state = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("GAME_MANAGER").TryGetComponent(out GameInfo GI);
        GI.TryGetComponent(out MusicManager BossMusicManager);
        BossMusicManager.IsInBoss = state;
        Destroy(this);
    }
}
