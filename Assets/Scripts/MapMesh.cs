using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapMesh : MonoBehaviour {

    public Tilemap map;

    private static Quaternion[] axisRotations = new Quaternion[5]
    {
        Quaternion.identity,
        Quaternion.Euler(90, 0, 0),
        Quaternion.Euler(-90, 0, 0),
        Quaternion.Euler(0, 90, 0),
        Quaternion.Euler(0, -90, 0)
    };

    private static Vector3Int[] directions = new Vector3Int[4]
    {
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0)
    };

    private static Vector3[] faceVertices = new Vector3[4]
    {
        new Vector3(-0.5f, -0.5f, 0f),
        new Vector3(0.5f, -0.5f, 0f),
        new Vector3(-0.5f, 0.5f, 0f),
        new Vector3(0.5f, 0.5f, 0f)
    };

    private static int[] faceTriangles = new int[6] { 0, 2, 1, 2, 3, 1 };


    List<Vector3> vertices = new List<Vector3>();
    List<Vector3> normals = new List<Vector3>();
    List<Vector3> uv = new List<Vector3>();
    List<int> triangles = new List<int>();
    
	void Start () {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filter.mesh = mesh;

        map.CompressBounds();
        
        for (int y = map.cellBounds.min.y; y < map.cellBounds.max.y; y++)
        {
            for (int x = map.cellBounds.min.x; x < map.cellBounds.max.x; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                Tile tile = map.GetTile(tilePosition) as Tile;
                if (tile == null) continue;

                // If it's protruding
                if (tile.colliderType == 0)
                {
                    MakeTile(new Vector3Int(x, y, 1), 0);

                    // In each direction, make a wall tile if it's bordered by something recessed
                    for (int dir_i = 0; dir_i < directions.Length; dir_i++)
                    {
                        Vector3Int offsetPosition = tilePosition + directions[dir_i];
                        Tile otherTile = map.GetTile(offsetPosition) as Tile;

                        if (otherTile != null && 
                            otherTile.colliderType != 0 &&
                            !IsOutsideMap(offsetPosition))
                        {
                            MakeTile(new Vector3Int(x, y, 1), dir_i + 1);
                        }
                    }
                }

                // If it's recessed
                else
                {
                    MakeTile(new Vector3Int(x, y, 0), 0);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        //mesh.uv = uv.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void MakeTile(Vector3Int position, int axis)
    {
        Quaternion rotation = axisRotations[axis];

        for (int i = 0; i < faceVertices.Length; i++)
        {
            Vector3 offset = axis == 0 ? 
                Vector3.zero : 
                (Vector3)directions[axis - 1] / 2 + new Vector3(0f, 0f, -0.5f);
            vertices.Add(rotation * faceVertices[i] + position + offset);
        }

        for (int i = 0; i < 4; i++)
        {
            normals.Add(rotation * -Vector3.forward);
        }

        int currentOffset = triangles.Count / 6 * 4;
        for (int i = 0; i < faceTriangles.Length; i++)
        {
            triangles.Add(faceTriangles[i] + currentOffset);
        }
    }

    private bool IsOutsideMap(Vector3Int position)
    {
        return position.x < map.cellBounds.min.x ||
            position.x >= map.cellBounds.max.x ||
            position.y < map.cellBounds.min.y ||
            position.y >= map.cellBounds.max.y;
    }
}
