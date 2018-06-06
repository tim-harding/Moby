using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CreateMobiusStripCollider : MonoBehaviour {

    public float MidcircleRadius = 1.0f;
    public float HalfWidth = 0.25f;
    public int Divisions = 256;

    void Start()
    {
        CreateMesh();
    }

    void OnValidate()
    {
        CreateMesh();
    }

    /// <summary>
    /// http://mathworld.wolfram.com/MoebiusStrip.html
    /// </summary>
    private void CreateMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[Divisions * 2];
        int[] triangles = new int[(Divisions * 2) * 3];
        Vector3[] normals = new Vector3[Divisions * 2];

        int vertsMod = Divisions * 2 - 1;

        for (int u = 0; u < Divisions; u++)
        {
            for (int v = 0; v < 2; v++)
            {
                float s = v == 0 ? -HalfWidth : HalfWidth;
                float t = (float)u / Divisions * Mathf.PI * 2.0f;
                float radial = MidcircleRadius + s * Mathf.Cos(0.5f * t);
                float x = radial * Mathf.Cos(t);
                float z = radial * Mathf.Sin(t);
                float y = s * Mathf.Sin(0.5f * t);

                int vert = (u * 2 + v);
                vertices[vert] = new Vector3(x, y, z);

                triangles[vert * 3] = (vert + v * 2) % vertsMod;
                triangles[vert * 3 + 1] = (vert + 1) % vertsMod;
                triangles[vert * 3 + 2] = (vert + 2 - v * 2) % vertsMod;
            }
        }

        for (int i = 0; i < Divisions; i++)
        {
            int vert = i * 2;
            Vector3 current = vertices[vert];
            Vector3 v1 = vertices[vert + 1] - current;
            Vector3 v2 = vertices[(vert + 2) % vertsMod] - current;
            Vector3 normal = -Vector3.Cross(v1.normalized, v2.normalized);
            normals[vert] = normal;
            normals[vert + 1] = normal;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.name = "Mobius Procedural";
        filter.mesh = mesh;
    }
}
