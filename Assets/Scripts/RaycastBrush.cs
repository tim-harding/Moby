using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBrush : MonoBehaviour {

    public Transform Target;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            bool hit = Physics.Raycast(ray, out hitInfo);
            if (hit)
            {
                Target.transform.position = hitInfo.point;
            }
        }
	}
}
