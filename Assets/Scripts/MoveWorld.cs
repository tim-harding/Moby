using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWorld : MonoBehaviour {

    public GameObject Player;
    public float Width = 128;
    private int WorldOffsetStep;
    private new Transform transform;
    private Transform playerTransform;

    private void Start()
    {
        transform = GetComponent<Transform>();
        playerTransform = Player.GetComponent<Transform>();
    }

    private void Update()
    {
        float offset = Width * WorldOffsetStep;
        float initialBounds = Width / 2;
        float worldUpper = initialBounds + offset;
        float worldLower = -initialBounds + offset;

        if (Player != null)
        {
            float pos = playerTransform.position.x;
            if (pos < worldLower || pos > worldUpper)
            {
                Move();
            }
        }
    }

    private void Move()
    {
        Vector3 playerPos = playerTransform.position;
        WorldOffsetStep = Mathf.FloorToInt((playerPos.x + Width / 2) / Width);
        Vector3 position = transform.localPosition;
        position.x = WorldOffsetStep * Width;
        transform.localPosition = position;

        Debug.Log("Did transform physics " + position);
    }
}
