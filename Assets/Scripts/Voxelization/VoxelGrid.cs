using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {

    public int XResolution = 16;
    public int YResolution = 16;
    
    private Voxel[,] cells;
    private bool NeedsRedraw = true;
    private MeshFilter filter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        
        cells = new Voxel[XResolution, YResolution];
        for (int x = 0; x < XResolution; x++)
        {
            for (int y = 0; y < YResolution; y++)
            {
                cells[x, y] = new Voxel(x, y);
            }
        }
    }

    void Update()
    {
        if (NeedsRedraw)
        {
            Triangulate();
            NeedsRedraw = false;
        }
    }

    public void PaintCircle(Vector2 center, float size, bool state)
    {
        Vector2Int halfRes = new Vector2Int(XResolution / 2, YResolution / 2);
        float squareSize = size * size;
        for (int y = (int)(center.y - size),
            yMax = (int)(center.y + size + 1);
            y < yMax;
            y++)
        {
            for (int x = (int)(center.x - size),
                xMax = (int)(center.x + size + 1);
                x < xMax;
                x++)
            {
                if (Mathf.Abs(x) < halfRes.x && Mathf.Abs(y) < halfRes.y)
                {
                    float xDifference = Mathf.Abs(center.x - x);
                    float yDifference = Mathf.Abs(center.y - y);
                    float distance = xDifference * xDifference + yDifference * yDifference;
                    if (distance < squareSize)
                    {
                        cells[x + halfRes.x, y + halfRes.y].state = state;
                    }
                }
            }
        }
        NeedsRedraw = true;
    }

    private void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < XResolution - 1; x++)
        {
            for (int y = 0; y < YResolution - 1; y++)
            {
                Voxel lowerLeft = cells[x, y];
                Voxel lowerRight = cells[x + 1, y];
                Voxel upperLeft = cells[x, y + 1];
                Voxel upperRight = cells[x + 1, y + 1];
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Marching Squares";
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        filter.mesh = mesh;
    }
}
