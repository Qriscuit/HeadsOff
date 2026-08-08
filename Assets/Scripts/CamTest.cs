using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    public float MouseY;
    public float MouseX;
    
    public float RotationSensitivity = 5;
    public float RotationY = 0;

    public float Sensitivity = 1;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    void Update()
    {
        Debug.Log(Input.GetAxisRaw("Mouse X"));


        MouseY = Input.GetAxis("Mouse Y");
        MouseX = Input.GetAxisRaw("Mouse X");

        RotationY = transform.eulerAngles.y;
        RotationY += MouseX * Time.deltaTime * RotationSensitivity;
        
        if(RotationY > 360)
            RotationY = 0;

        if (RotationY < -360)
            RotationY = 0;

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Sensitivity * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Sensitivity * Time.deltaTime;
        }

        //Vector3 Rot = Vector3.Lerp(transform.eulerAngles, new Vector3(transform.eulerAngles.x, RotationY, transform.eulerAngles.z), 0.5f);

        Quaternion ROT = Quaternion.Euler(transform.eulerAngles.x, RotationY, transform.eulerAngles.z);

        transform.rotation = Quaternion.Slerp(transform.rotation, ROT, 1f);
    }
}
