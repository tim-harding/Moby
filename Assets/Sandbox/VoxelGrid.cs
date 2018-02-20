using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal struct Voxel
{
    public bool Mode;
    public Vector2 Position;

    // Indices into vertex list, once they have been used
    public int Base;
    public int YEdge;
    public int XEdge;

    public Voxel(Vector2 position, bool mode)
    {
        Mode = mode;
        Position = position;
        Base = -1;
        YEdge = -1;
        XEdge = -1;
    }
}

public class VoxelGrid : MonoBehaviour {

    public int XResolution = 16;
    public int YResolution = 16;
    
    private Voxel[] Cells;
    private bool NeedsRedraw = true;
    private MeshFilter filter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<Vector3> normals = new List<Vector3>();
    private List<int> triangles = new List<int>();

    void Start()
    {
        filter = GetComponent<MeshFilter>();

        int cellsSize = XResolution * YResolution;
        Cells = new Voxel[cellsSize];
        for (int cell = 0; cell < cellsSize; cell++)
        {
            Cells[cell] = new Voxel();
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
                    // Don't use GetCell because this is the only place I really need index bounds checking
                    int index = y * XResolution + x;
                    if (index < Cells.Length && index > 0)
                    {
                        Cells[index].Mode = mode;
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

        for (int cell = 0; cell < Cells.Length; cell++)
        {
            bool mode = Cells[cell].Mode;
            Vector2 position = new Vector2(
                cell % XResolution - XResolution / 2,
                cell / XResolution - YResolution / 2
            );
            Voxel voxel = new Voxel(position, mode);
            Cells[cell] = voxel;
        }

        vertices.Clear();
        normals.Clear();
        triangles.Clear();

        for (int x = 0; x < XResolution - 1; x++)
        {
            for (int y = 0; y < YResolution - 1; y++)
            {
                Voxel lowerLeft = GetCell(x, y);
                Voxel lowerRight = GetCell(x + 1, y);
                Voxel upperLeft = GetCell(x, y + 1);
                Voxel upperRight = GetCell(x + 1, y + 1);
                
                int cellType = lowerLeft.Mode ? 1 : 0;
                cellType |= lowerRight.Mode ? 2 : 0;
                cellType |= upperLeft.Mode ? 4 : 0;
                cellType |= upperRight.Mode ? 8 : 0;

                switch (cellType)
                {
                    case 0:
                        break;
                    default:
                        SetUpVoxels(lowerLeft);
                        Triangle(lowerLeft.Base, lowerLeft.YEdge, lowerLeft.XEdge);
                        break;
                    //case 2:
                    //    SetUpVoxels(lowerLeft, lowerRight);
                    //    Triangle(lowerLeft.XEdge, lowerRight.YEdge, lowerRight.Base);
                    //    break;
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        for (int i = 0; i < vertices.Count; i++)
        {
            normals.Add(Vector3.forward);
        }
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        filter.mesh = mesh;
    }

    private void Triangle(int first, int second, int third)
    {
        triangles.Add(first);
        triangles.Add(second);
        triangles.Add(third);
    }

    private void SetUpVoxels(params Voxel[] voxels)
    {
        foreach (Voxel voxel in voxels)
        {
            SetUpVoxel(voxel);
        }
    }

    private void SetUpVoxel(Voxel voxel)
    {
        SetUpVoxelIndex(ref voxel.Base, voxel.Position);
        SetUpVoxelIndex(ref voxel.XEdge, voxel.Position + new Vector2(0.5f, 0f));
        SetUpVoxelIndex(ref voxel.YEdge, voxel.Position + new Vector2(0f, 0.5f));
    }

    private void SetUpVoxelIndex(ref int index, Vector2 position)
    {
        if (index == -1)
        {
            index = vertices.Count;
            vertices.Add(new Vector3(position.x - XResolution / 2, position.y - YResolution / 2, 0));
        }
    }

    private Voxel GetCell(int x, int y)
    {
        return Cells[y * XResolution + x];
    }
}
