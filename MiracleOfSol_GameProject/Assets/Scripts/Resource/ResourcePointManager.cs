using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePointManager : MonoBehaviour
{
    private CapturePoint[] AllCP;
    private BasicInfo[] CPInfo;

    // Start is called before the first frame update
    void Start()
    {
        AllCP = new CapturePoint[transform.childCount];
        CPInfo = new BasicInfo[transform.childCount];
        for (int CP = 0; CP < transform.childCount; CP++)
        {
            GameObject child = transform.GetChild(CP).gameObject;
            AllCP[CP] = child.GetComponent<CapturePoint>();
            CPInfo[CP] = child.GetComponent<BasicInfo>();
        }
    }

    public CapturePoint[] GetAllCP()
    {
        return AllCP;
    }

    public BasicInfo[] GetAllCP_BI()
    {
        return CPInfo;
    }
}
