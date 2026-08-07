using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    public float CardinalMoveSpeed;
    public float VerticalMoveSpeed;

    public Camera _MC;
    public Transform LookAtObject;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _MC.transform.LookAt(LookAtObject);

        //transform.forward = _MC.transform.forward;

        if (Input.GetKey(KeyCode.W))
        {
            transform.localPosition += transform.InverseTransformVector(Vector3.forward * CardinalMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.InverseTransformVector(Vector3.right * CardinalMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.InverseTransformVector(Vector3.forward * CardinalMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.InverseTransformVector(Vector3.right * CardinalMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            transform.position -= transform.InverseTransformVector(Vector3.up * CardinalMoveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.position += transform.InverseTransformVector(Vector3.up * CardinalMoveSpeed * Time.deltaTime);
        }
    }
}
