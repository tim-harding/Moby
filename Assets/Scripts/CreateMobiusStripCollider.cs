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
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.sharedMesh = GetComponent<MeshFilter>().mesh;
        }
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

        int vertCount = (Divisions + 1) * 2;
        Vector3[] vertices = new Vector3[vertCount];
        int[] triangles = new int[(Divisions * 2) * 3];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];

        for (int u = 0; u < Divisions + 1; u++)
        {
            for (int v = 0; v < 2; v++)
            {
                float s = v == 0 ? -HalfWidth : HalfWidth;
                float t = (float)u / Divisions * Mathf.PI * 4.0f;
                float radial = MidcircleRadius + s * Mathf.Cos(0.5f * t);
                float x = radial * Mathf.Cos(t);
                float z = radial * Mathf.Sin(t);
                float y = s * Mathf.Sin(0.5f * t);

                int vert = (u * 2 + v);
                vertices[vert] = new Vector3(x, y, z);
                uv[vert] = new Vector2((float)u / Divisions, v);

                if (u < Divisions)
                {
                    triangles[vert * 3] = vert + v * 2;
                    triangles[vert * 3 + 1] = vert + 1;
                    triangles[vert * 3 + 2] = vert + 2 - v * 2;
                }
            }
        }

        for (int i = 0; i < Divisions; i++)
        {
            int vert = i * 2;
            Vector3 current = vertices[vert];
            Vector3 v1 = vertices[vert + 1] - current;
            Vector3 v2 = vertices[(vert + 2)] - current;
            Vector3 normal = Vector3.Cross(v1, v2);
            normals[vert] = normal;
            normals[vert + 1] = normal;
        }

        Vector3 lastNormal = normals[normals.Length - 3];
        normals[normals.Length - 2] = lastNormal;
        normals[normals.Length - 1] = lastNormal;

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.name = "Mobius Procedural";
        filter.mesh = mesh;
    }
}
