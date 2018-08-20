using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {


    //Generic Bookkeeping vars
    private Vector3 lastMousePosition;  //From input.mouseposition

    //Camera Dragging bookkeeping vars
    public bool MouseCameraControl = true;

    private Vector3 LastMouseGroundPlanePosition;

    delegate void UpdateFunc();
    private UpdateFunc Update_CurrentFunc;


    // Use this for initialization
    void Start () {
        Update_CurrentFunc = Update_DetectModeStart;
    }
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.Escape)) {
            //Cancel anaything
            CancelUpdateFunc();
	    }

	    Update_CurrentFunc();

	    lastMousePosition = Input.mousePosition;
	}

    virtual public Vector3 MouseToGroundPlane()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //What is the point at which the mouse ray intersects Y=0

        if (mouseRay.direction.y >= 0)
        {
            //Util.Util.WriteDebugLog("Why is mouse pointing up?", 4, true, 4);
            //return;
        }

        float rayLength = (mouseRay.origin.y / mouseRay.direction.y);
        return mouseRay.origin - (mouseRay.direction * rayLength);
    }

    void Update_DetectModeStart() {
        if (Input.GetMouseButtonDown(0)) {
            //Left Mouse button is down
            //This doesn't do anything by itself, depends if we're draging etc

        }
        else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePosition ){
            //Left button is being held down and the mouse moved that's a camera drag
            Update_CurrentFunc = Update_CameraDrag;
            Update_CurrentFunc();
        }

    }

    void CancelUpdateFunc() {
        Update_CurrentFunc = Update_DetectModeStart;

        //TODO Also do any ui cleanup 
    }

    virtual public void Update_CameraDrag() {

        if (Input.GetMouseButtonUp(0)) {
            CancelUpdateFunc();
            return;
        }

        //Util.Util.WriteDebugLog("MouseControl", 4, true,4);
        var hitPos = MouseToGroundPlane();

        //Debug.Log(string.Format("HitPos: {0}", hitPos));

        Vector3 diff = LastMouseGroundPlanePosition - hitPos;
        Camera.main.transform.Translate(diff, Space.World);
        //hitPos = HitPos();
        LastMouseGroundPlanePosition = MouseToGroundPlane();

    }

}
