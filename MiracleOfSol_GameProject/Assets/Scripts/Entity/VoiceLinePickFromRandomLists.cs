using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLinePickFromRandomLists : MonoBehaviour
{
    public AudioClip[] LinesType01_Attack;
    public AudioClip[] LinesType02_Attack;
    public AudioClip[] LinesType03_Attack;
    public AudioClip[] LinesType04_Attack;
    public AudioClip[] LinesType05_Attack;
    public AudioClip[] LinesType06_Attack;
    public AudioClip[] LinesType07_Attack;
    public AudioClip[] LinesType08_Attack;
    public AudioClip[] LinesType09_Attack;
    public AudioClip[] LinesType01_Die;
    public AudioClip[] LinesType02_Die;
    public AudioClip[] LinesType03_Die;
    public AudioClip[] LinesType04_Die;
    public AudioClip[] LinesType05_Die;
    public AudioClip[] LinesType06_Die;
    public AudioClip[] LinesType07_Die;
    public AudioClip[] LinesType08_Die;
    public AudioClip[] LinesType09_Die;


    // Start is called before the first frame update
    void Start()
    {
        AudioClip[][] TmpAllAttack = new AudioClip[9][]{ LinesType01_Attack,LinesType02_Attack,LinesType03_Attack,
                                                        LinesType04_Attack,LinesType05_Attack,LinesType06_Attack,
                                                        LinesType07_Attack,LinesType08_Attack,LinesType09_Attack };
        AudioClip[][] TmpAllDie = new AudioClip[9][]{ LinesType01_Die,LinesType02_Die,LinesType03_Die,
                                                        LinesType04_Die,LinesType05_Die,LinesType06_Die,
                                                        LinesType07_Die,LinesType08_Die,LinesType09_Die };
        int AllLinesLength = 0;
        for (int i = 0; i < TmpAllAttack.Length; i++)
        {
            if (TmpAllAttack[i].Length == 0)
            {
                AllLinesLength = i-1;
                break;
            }
        }

        gameObject.TryGetComponent(out VoiceLineManager tmp_VLM);
        int rdmLinePos = Random.Range(0, AllLinesLength);
        tmp_VLM.AttackLines = TmpAllAttack[rdmLinePos];
        tmp_VLM.DieLines = TmpAllDie[rdmLinePos];

        Destroy(this);
    }
}
