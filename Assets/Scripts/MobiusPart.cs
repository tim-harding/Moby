using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobiusPart : MonoBehaviour {
    
    private float CurrentX = 0f;

    public float Width = 256;
    public float FollowSpeed = 2f;
    public GameObject FollowObject;
    public Material DeformerMaterial;

    private void Start()
    {
        CurrentX = FollowObject.transform.position.x;
        GetComponent<MeshRenderer>().sharedMaterial = DeformerMaterial;
    }

    void Update () {
        float DstX = FollowObject.transform.position.x;

        CurrentX = Mathf.Lerp(CurrentX, DstX, FollowSpeed * Time.deltaTime);

        DeformerMaterial.SetFloat("_Offset", -CurrentX);
	}
}
