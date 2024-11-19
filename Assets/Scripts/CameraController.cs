using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool debugs = true;
    Vector3 myLook;
    float lookSpeed = 100f;
    public Camera myCam;
    public float camLock = 90f;
    float onStartTimer;
    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        myLook = transform.localEulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        onStartTimer += Time.deltaTime;
        Vector3 delta = DeltaLook() * Time.deltaTime;
        myLook += DeltaLook() * Time.deltaTime;
        myLook.y = Mathf.Clamp(myLook.y, -camLock, camLock);

        //Debug.Log("myLook: " + myLook);
        transform.rotation = Quaternion.Euler(0f, myLook.x, 0f);
        myCam.transform.rotation = Quaternion.Euler(-myLook.y, myLook.x, 0f);

        if (debugs)
        {
            Debug.DrawRay(myCam.transform.position, myCam.transform.forward * 10f, Color.black);
        }
    }

    Vector3 DeltaLook()
    {
        Vector3 dLook;
        float rotY = Input.GetAxis("Mouse Y") * lookSpeed;
        float rotX = Input.GetAxis("Mouse X") * lookSpeed;
        dLook = new Vector3(rotX, rotY, 0f);
        if (dLook != Vector3.zero)
        {
            //Debug.Log("delta look: " + dLook);
        }
        if (onStartTimer < 1f)
        {
            dLook = Vector3.ClampMagnitude(dLook, onStartTimer * 10f);
        }

        return dLook;
    }
}