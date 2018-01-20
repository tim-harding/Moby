using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HideOnPlay : MonoBehaviour {
    
	void Start () {
        GetComponent<Renderer>().enabled = false;
	}
}
