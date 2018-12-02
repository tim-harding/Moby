using System;
using UnityEngine;

[Serializable]
public class Voxel
{
    public bool state;
    public Vector2 position;
    public Vector2 xEdgePosition;
    public Vector2 yEdgePosition;

    public Voxel(int x, int y)
    {
        position.x = x + 0.5f;
        position.y = y + 0.5f;
        xEdgePosition = position;
        yEdgePosition = position;
        xEdgePosition.x += 0.5f;
        xEdgePosition.y += 0.5f;
    }
}