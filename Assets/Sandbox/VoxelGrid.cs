using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid : MonoBehaviour {

    public int XResolution = 16;
    public int YResolution = 16;
    
    private Voxel[,] Cells;
    private bool NeedsRedraw = true;
    private MeshFilter filter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        
        Cells = new Voxel[XResolution, YResolution];
        for (int x = 0; x < XResolution; x++)
        {
            for (int y = 0; y < YResolution; y++)
            {
                Cells[x, y] = new Voxel(false);
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

    public void PaintCircle(Vector2 center, float size, bool mode)
    {
        Vector2Int halfRes = new Vector2Int(XResolution / 2, YResolution / 2);
        float squareSize = size * size;
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
                if (Mathf.Abs(x) < halfRes.x && Mathf.Abs(y) < halfRes.y)
                {
                    float xDifference = Mathf.Abs(center.x - x);
                    float yDifference = Mathf.Abs(center.y - y);
                    float distance = xDifference * xDifference + yDifference * yDifference;
                    if (distance < squareSize)
                    {
                        Cells[x + halfRes.x, y + halfRes.y].Mode = mode;
                    }
                }
            }
        }
        NeedsRedraw = true;
    }

    private void Triangulate()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Marching Squares";

        for (int x = 0; x < XResolution; x++)
        {
            for (int y = 0; y < YResolution; y++)
            {
                Cells[x, y].ResetIndices();
            }
        }

        vertices.Clear();
        normals.Clear();
        triangles.Clear();

        for (int x = 0; x < XResolution - 1; x++)
        {
            for (int y = 0; y < YResolution - 1; y++)
            {
                Voxel lowerLeft = Cells[x, y];
                Voxel lowerRight = Cells[x + 1, y];
                Voxel upperLeft = Cells[x, y + 1];
                Voxel upperRight = Cells[x + 1, y + 1];
                
                int cellType = lowerLeft.Mode ? 1 : 0;
                cellType |= lowerRight.Mode ? 2 : 0;
                cellType |= upperLeft.Mode ? 4 : 0;
                cellType |= upperRight.Mode ? 8 : 0;

                switch (cellType)
                {
                    case 0:
                        break;
                    default:
                        AddVert(x, y, 0, ref lowerLeft.Base);
                        AddVert(x, y + 0.5f, 0, ref lowerLeft.YEdge);
                        AddVert(x + 0.5f, y, 0, ref lowerLeft.XEdge);
                        break;
                }
            }
        }

        for (int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.forward);
        }
        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        filter.mesh = mesh;
    }

    private void AddVert(float x, float y, float z, ref int index)
    {
        if (index == -1)
        {
            index = vertices.Count;
            vertices.Add(new Vector3(x - XResolution / 2, y - YResolution / 2, z));
        }
        triangles.Add(index);
    }
}
