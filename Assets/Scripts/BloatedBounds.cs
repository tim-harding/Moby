using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prevents frustum culling of geometry displaced by the shader
/// </summary>
public class BloatedBounds : MonoBehaviour {
    
	void Start () {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(1000f, 1000f, 1000f));
        mesh.bounds = bounds;
	}
}
