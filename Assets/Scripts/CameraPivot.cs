using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPivot : MonoBehaviour {

    public float FollowSpeed = 2;
    public float Sensitivity = 1;

    private Vector2 TargetRotation = new Vector2(0, 0);
    private Vector2 CurrentRotation = new Vector2(0, 0);
    private new Transform transform;

    private void Start()
    {
        transform = GetComponent<Transform>();
    }

    void Update() {
        TargetRotation.x = TargetRotation.x - Input.GetAxis("RightHorizontal") * Sensitivity;
        TargetRotation.y = Mathf.Clamp(TargetRotation.y - Input.GetAxis("RightVertical") * Sensitivity, -90, 90);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            CurrentRotation.x = Mathf.Repeat(CurrentRotation.x + 180, 360) - 180;
            TargetRotation = new Vector2(0, 0);
        }

        CurrentRotation = Vector2.Lerp(CurrentRotation, TargetRotation, FollowSpeed * Time.deltaTime);

        Quaternion yRot = Quaternion.Euler(0, CurrentRotation.x, 0);
        Quaternion xRot = Quaternion.Euler(CurrentRotation.y, 0, 0);

        transform.rotation = yRot * xRot;
	}
}
