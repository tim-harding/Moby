using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{

    public float minGroundNormalY = .95f;
    public float gravityModifier = 1f;

    protected Vector2 TargetVelocity;
    protected bool Grounded;
    protected Vector2 GroundNormal;
    protected Collider2D Collider;
    protected Vector2 Velocity;
    protected ContactFilter2D ContactFilter;
    protected RaycastHit2D[] HitBuffer = new RaycastHit2D[4];

    protected const float MinMoveDistance = 0.001f;
    protected const float ShellRadius = 0.01f;
    
    protected virtual void Setup() { }
    protected virtual void ComputeVelocity() { }

    void Start()
    {
        Collider = GetComponent<Collider2D>();
        ContactFilter.useTriggers = false;
        ContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        ContactFilter.useLayerMask = true;
        Setup();
    }


    void Update()
    {
        TargetVelocity = Vector2.zero;
        ComputeVelocity();

        Velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        Velocity.x = TargetVelocity.x;
        Grounded = false;
        Vector2 deltaPosition = Velocity * Time.deltaTime;
        Vector2 moveAlongGround = new Vector2(GroundNormal.y, -GroundNormal.x);
        Vector2 move = moveAlongGround * deltaPosition.x;
        Movement(move, false);
        move = Vector2.up * deltaPosition.y;
        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement)
    {
        float distance = move.magnitude;

        if (distance > MinMoveDistance)
        {
            int count = Collider.Cast(move, ContactFilter, HitBuffer, distance + ShellRadius);

            for (int i = 0; i < count; i++)
            {
                Vector2 currentNormal = HitBuffer[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    Grounded = true;
                    if (yMovement)
                    {
                        GroundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(Velocity, currentNormal);
                if (projection < 0)
                {
                    Velocity = Velocity - projection * currentNormal;
                }

                float modifiedDistance = HitBuffer[i].distance - ShellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

        Vector3 pos = transform.position;
        Vector2 movement = move.normalized * distance;
        pos += new Vector3(movement.x, movement.y, 0);
        transform.position = pos;
    }

}