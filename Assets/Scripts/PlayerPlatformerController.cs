using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject
{

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;

    public float accelerate = 0.3f;
    public float decelerate = 0.8f;
    public float airborneModifier = 0.2f;
    public float respawnHeight = -10f;

    private Vector3 StartPosition;
    private Vector2 CurrentTarget;

    protected override void Setup()
    {
        StartPosition = transform.position;
    }

    protected override void ComputeVelocity()
    {
        if (transform.position.y < respawnHeight)
        {
            transform.position = StartPosition;
            CurrentTarget = Vector2.zero;
        }

        Vector2 move = Vector2.zero;

        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal > Velocity.x)
        {
            float adjustedAccel = Grounded ? accelerate : accelerate * airborneModifier;
            CurrentTarget.x = Mathf.Lerp(CurrentTarget.x, horizontal, adjustedAccel);
        }
        else
        {
            float adjustedAccel = Grounded ? decelerate : decelerate * airborneModifier;
            CurrentTarget.x = Mathf.Lerp(CurrentTarget.x, horizontal, adjustedAccel);
        }

        // Go to sleep if moving slowly
        if (Mathf.Abs(CurrentTarget.x) > 0.01)
        {
            move.x = CurrentTarget.x;
        }

        if (Input.GetButtonDown("Jump") && Grounded)
        {
            Velocity.y = jumpTakeOffSpeed;
        }
        else if (Input.GetButtonUp("Jump"))
        {
            if (Velocity.y > 0)
            {
                Velocity.y = Velocity.y * 0.5f;
            }
        }

        TargetVelocity = move * maxSpeed;
    }
}