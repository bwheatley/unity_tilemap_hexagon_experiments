using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjectNamePlate : MonoBehaviour {

    public GameObject MyTarget;
    public Vector3 ScreenPositionOffset = new Vector3(0,30,0);
    public Vector3 WorldPositionOffset = new Vector3(0,1,0);

    public Camera TheCamera;

    private RectTransform rectTransform;

	// Use this for initialization
	void Start () {
	    if (TheCamera == null) {
            TheCamera = Camera.main;
	    }

	    rectTransform = GetComponent<RectTransform>();


	}
	
	// Update is called once per frame
	void LateUpdate () {
	    if (MyTarget == null) {
            // The object we're trying to follow is gone, so let's destroy oursleves
	        Destroy(gameObject);
	        return;
	    }

		//Find out the screen position of our Object and set ourselves to that plus offset.
	    Vector3 screenPos = TheCamera.WorldToScreenPoint(MyTarget.transform.position + WorldPositionOffset);

	    rectTransform.anchoredPosition = screenPos + ScreenPositionOffset;
	}
}
