using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    public Transform MMCam;
    public bool Active = false;
    public float PanSpeed = 20;
    public float PanBorderThickness = 50;
    public Vector3 PanLimit;

    public float ScrollSpeed = 2;
    public float RotationSensitivity = 3;

    public KeyCode[] AcceptedPanKeys = new KeyCode[4] { KeyCode.UpArrow, KeyCode.RightArrow, KeyCode.DownArrow, KeyCode.LeftArrow };

    public LayerMask FloorLayer;


    private Vector3[] PanDirection = new Vector3[4] { new Vector3(0, 0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0) };
    private Vector3 StartingPos;
    private Quaternion CameraStartingAngle;
    private Vector3 Mouse_Location;
    private Vector3 FocusOffset = new Vector3(-15, 10, 11);
    private float Camera_Sensitivity = 1;
    private Transform EntityToFollow = null;
    private bool IsFully3D;

    private void Start()
    {
        StartingPos = transform.position;
        CameraStartingAngle = Camera.main.transform.localRotation;
        FloorLayer = LayerMask.GetMask("Terrain");
        InvokeRepeating(nameof(UpdateGlobals), 0, 1);
        Active = true;
    }

    private void UpdateGlobals()
    {
        IsFully3D = PlayerPrefsX.GetBool("Is3DCamera", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) { Active = !Active; }
        PanDirection[0] = Vector3.Scale(transform.forward, new Vector3(1,0,1)); PanDirection[1] = transform.right; PanDirection[2] = Vector3.Scale(-transform.forward, new Vector3(1, 0, 1)); PanDirection[3] = -transform.right;
        
        if (Active)
        {
            float Scroll = Input.GetAxis("Mouse ScrollWheel");
            Camera.main.transform.localPosition = Camera.main.transform.localPosition + Scroll * ScrollSpeed * 100 * Time.deltaTime * Camera.main.transform.forward;
            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, Mathf.Clamp(Camera.main.transform.localPosition.y, -30 + StartingPos.y, PanLimit[1] + StartingPos.y), Camera.main.transform.localPosition.z);

            for (int i = 0; i < AcceptedPanKeys.Length; i++)
            {
                if (Input.GetKey(AcceptedPanKeys[i]) || CheckPanFromMouse(i))
                {
                    PanCamera(i);
                }
            }

            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetMouseButton(3)) && IsFully3D)
            {
                //Taken from Camera Controller Script by ZeroPunchProductions
                // https://assetstore.unity.com/packages/tools/camera/simple-camera-controller-159957
                Mouse_Location = Input.mousePosition - Mouse_Location;
                Mouse_Location = new Vector3(-Mouse_Location.y * Camera_Sensitivity, Mouse_Location.x * Camera_Sensitivity, 0);
                Mouse_Location = new Vector3(transform.GetChild(0).eulerAngles.x + Mouse_Location.x, transform.GetChild(0).eulerAngles.y + Mouse_Location.y, 0);
                //End of Camera Controller Script

                Camera.main.transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(Camera.main.transform.eulerAngles), Quaternion.Euler(Mouse_Location), Time.deltaTime * RotationSensitivity * 100);
                //MMCam.rotation = Quaternion.Euler(MMCam.eulerAngles[0], MMCam.eulerAngles[1], Camera.main.transform.eulerAngles[1]);
                Mouse_Location = Input.mousePosition;
            }

            if (Input.GetKey(KeyCode.Backspace))
            {
                ResetCameraAngle();
                //MMCam.rotation = Quaternion.Euler(MMCam.eulerAngles[0], MMCam.eulerAngles[1], Camera.main.transform.eulerAngles[1]);
            }

            if(EntityToFollow != null)
            {
                FocusOnPosition(EntityToFollow.position, false);
            }
        }
    }


    private void ResetCameraAngle(bool ResetEntityTracking = true)
    {
        if (ResetEntityTracking)
        {
            EntityToFollow = null;
        }

        transform.position = new Vector3(transform.position.x, StartingPos.y, transform.position.z);
        Camera.main.transform.localRotation = CameraStartingAngle;
    }


    private void PanCamera(int Direction)
    {
        Vector3 MoveDistance = transform.position + PanSpeed * PanDirection[Direction] * Time.deltaTime;
        EntityToFollow = null;

        for(int i = 0; i < 3; i++)
        {
            if (i != 1) //Don't check height here.
            {
                MoveDistance[i] = Mathf.Clamp(MoveDistance[i], -PanLimit[i], PanLimit[i]);
            }
            else
            {
                MoveDistance[i] = Mathf.Clamp(MoveDistance[i], 1, PanLimit[i] + StartingPos[i]);
            }
        }

        transform.position = MoveDistance;        
    }

    private bool CheckPanFromMouse(int Direction)
    {
        float[] MousePos = new float[4] { Input.mousePosition.y, Input.mousePosition.x, -Input.mousePosition.y, -Input.mousePosition.x };
        float[] ScreenSize = new float[4] { Screen.height, Screen.width, PanBorderThickness, PanBorderThickness };

        if (MousePos[Direction] >= ScreenSize[Direction] - PanBorderThickness && MousePos[Direction] <= ScreenSize[Direction] + PanBorderThickness/2) 
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FocusOnEntity(Transform Entity)
    {
        EntityToFollow = Entity;
        FocusOnPosition(Entity.position, false);
    }

    public void FocusOnPosition(Vector3 EntityPos, bool ResetRotation = true)
    {
        if (ResetRotation) { ResetCameraAngle(false); } 
        else { transform.LookAt(EntityPos); }
        transform.position = EntityPos + FocusOffset;
    }

}
