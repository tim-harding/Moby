using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {

    public int XResolution = 16;
    public int YResolution = 16;
    
    private bool[] Cells;
    private bool NeedsRedraw = true;
    private MeshFilter filter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<int> trianglesOn = new List<int>();
    private List<int> trianglesOff = new List<int>();

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        Cells = new bool[XResolution * YResolution];
    }

    void Update()
    {
        if (NeedsRedraw)
        {
            RefreshGrid();
        }
    }

    public void PaintCircle(Vector2 center, float size, bool mode)
    {
        center += new Vector2(XResolution / 2, YResolution / 2);
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
                float xDifference = Mathf.Abs(center.x - x);
                float yDifference = Mathf.Abs(center.y - y);
                float distance = xDifference * xDifference + yDifference * yDifference;
                if (distance < size * size)
                {
                    int index = y * XResolution + x;
                    if (index < Cells.Length && index > 0)
                    {
                        Cells[index] = mode;
                    }
                }
            }
        }
        NeedsRedraw = true;
    }

    private void RefreshGrid()
    {
        if (filter == null)
        {
            Debug.Log("Filter is null.");
            return;
        }

        Mesh mesh = new Mesh();
        mesh.name = "Voxel Preview Mesh";
        mesh.subMeshCount = 2;

        vertices.Clear();
        normals.Clear();
        trianglesOn.Clear();
        trianglesOff.Clear();

        for (int x = 0; x < XResolution; x++)
        {
            for (int y = 0; y < YResolution; y++)
            {
                MakeQuad(new Vector3(x - XResolution / 2, y - YResolution / 2, 0), 0.2f, Cells[y * XResolution + x]);
            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.SetTriangles(trianglesOn, 0);
        mesh.SetTriangles(trianglesOff, 1);
        filter.mesh = mesh;

        NeedsRedraw = false;
    }

    private void MakeQuad(Vector3 center, float size, bool mode)
    {
        vertices.Add(center + new Vector3(-size, -size, 0));
        vertices.Add(center + new Vector3(-size, size, 0));
        vertices.Add(center + new Vector3(size, size, 0));
        vertices.Add(center + new Vector3(size, -size, 0));

        for (int i = 0; i < 4; i++)
        {
            normals.Add(-Vector3.forward);
        }

        List<int> triangles = mode ? trianglesOn : trianglesOff;

        int count = vertices.Count;
        triangles.Add(count - 4);
        triangles.Add(count - 3);
        triangles.Add(count - 1);
        triangles.Add(count - 3);
        triangles.Add(count - 2);
        triangles.Add(count - 1);
    }
}
