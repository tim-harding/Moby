using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewportCamera : MonoBehaviour
{
    public float MoveSpeed = 1.0f;
    public float RotateSpeed = 1.0f;

    private float RotateY = 0.0f;
    private float RotateX = 0.0f;

    private void Start()
    {
        RotateY = transform.localEulerAngles.y;
        RotateX = transform.localEulerAngles.x;
    }

    private void Update()
    {
        float hori = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        Vector3 lateral = Vector3.Cross(transform.up, transform.forward);
        transform.position += lateral * hori * MoveSpeed;
        transform.position += transform.forward * vert * MoveSpeed;
        transform.position += new Vector3(0, 1, 0) * Input.GetAxis("Dolly") * MoveSpeed;

        if (Input.GetMouseButton(1))
        {
            RotateY += Input.GetAxis("Mouse X") * RotateSpeed;
            RotateX += Input.GetAxis("Mouse Y") * RotateSpeed * -1.0f;

            transform.rotation = Quaternion.Euler(RotateX, RotateY, 0);
        }
    }
}