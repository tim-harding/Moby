using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBrush : MonoBehaviour {
    
    public VoxelGrid target;

    private const uint BrushDivisions = 6;
    private float BrushScale = 1f;
    private bool Mode = true;
    private MeshFilter filter;
    private new Transform transform;
    private new MeshRenderer renderer;
    private bool NeedsRedraw;
    private Vector2 PreviousMousePosition;

    void Start()
    {
        filter = GetComponent<MeshFilter>();
        transform = GetComponent<Transform>();
        renderer = GetComponent<MeshRenderer>();
        UpdateBrushMesh();
    }

    void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            BrushScale += (mousePosition.x - PreviousMousePosition.x) / 100f;
            float scale = BrushScale * BrushScale;
            transform.localScale = new Vector3(scale, scale, scale);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            float scalar = -ray.origin.z / ray.direction.z;
            Vector3 zIntersect = ray.origin + ray.direction * scalar;
            zIntersect.z -= 0.1f;
            transform.position = zIntersect;

            if (Input.GetKeyDown(KeyCode.X))
            {
                Mode = !Mode;
                float value = Mode ? 1f : 0f;
                renderer.sharedMaterial.color = new Color(value, value, value, 0.4f);
            }

            if (Input.GetMouseButton(0))
            {
                target.PaintCircle(new Vector2(zIntersect.x, zIntersect.y), BrushScale * BrushScale, Mode);
            }
        }
        PreviousMousePosition = mousePosition;
    }

    void UpdateBrushMesh()
    {
        int slices = 1 << (int)BrushDivisions; // Powers of 2
        Vector3[] vertices = new Vector3[slices + 1];
        Vector3[] normals = new Vector3[slices + 1];
        int[] triangles = new int[slices * 3];

        for (int slice = 0; slice < slices; slice++)
        {
            float radian = Mathf.PI * 2 * slice / slices;
            vertices[slice] = new Vector3(
                Mathf.Cos(radian),
                Mathf.Sin(radian),
                0f
            );

            normals[slice] = -Vector3.forward;

            int triangleOffset = slice * 3;
            triangles[triangleOffset] = slice + 1;
            triangles[triangleOffset + 1] = slice;
            triangles[triangleOffset + 2] = slices;
        }
        vertices[slices] = Vector3.zero;
        normals[slices] = Vector3.forward;
        triangles[(slices - 1) * 3] = 0;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.triangles = triangles;
        filter.mesh = mesh;
    }
}
