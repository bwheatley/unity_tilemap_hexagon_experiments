using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController.Util;

public class MouseController : MonoBehaviour {


    //Generic Bookkeeping vars
    private Vector3 lastMousePosition;  //From input.mouseposition

    //Camera Dragging bookkeeping vars
    public bool MouseCameraControl = true;
    private int mouseDragThreshold = 1;

    private Vector3 LastMouseGroundPlanePosition;

    delegate void UpdateFunc();
    private UpdateFunc Update_CurrentFunc;

    private Unit selectedUnit = null;



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
        else if(Input.GetMouseButtonUp(0)){
            //Select unit
        }
        // Mouse camera broke for now fix later
        //else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePosition)
        //{
        //    //todo add in a threshold to reduce folks with jittery hands
        //    //Left button is being held down and the mouse moved that's a camera drag
        //    Update_CurrentFunc = Update_CameraDrag;
        //    Update_CurrentFunc();
        //}
        else if (selectedUnit != null && Input.GetMouseButton(1)) {
            //We have a selected unit and are holding down the mouse mbutton, show a path from 
            // unit to mouse position via the pathfinding system



        }

    }

    void Update_UnitMovement() {
        if (Input.GetMouseButtonUp(1)) {
            Util.WriteDebugLog(string.Format("Complete Unit movements"), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);

            //Todo copy pathfinding path to units movement queue

            Cancel();

        }
    }

    void Cancel(string message = null) {
        if (message != null) {
            Util.WriteDebugLog(string.Format("{0}", message), GameManager.LogLevel_Info, GameManager.instance.debug,
                GameManager.instance.LogLevel);
        }

        CancelUpdateFunc();
        return;
    }

    void CancelUpdateFunc() {
        Update_CurrentFunc = Update_DetectModeStart;

        //TODO Also do any ui cleanup 
    }

    virtual public void Update_CameraDrag() {

        if (Input.GetMouseButtonUp(0)) {
            Cancel("Cancel Camera Drag");
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
