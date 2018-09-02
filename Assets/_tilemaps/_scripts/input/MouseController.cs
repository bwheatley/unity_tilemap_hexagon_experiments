using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController.Util;
using UnityEngine.EventSystems;

public class MouseController : MonoBehaviour {


    //Generic Bookkeeping vars
    private Vector3 lastMousePosition;  //From input.mouseposition
    HexMap hexMap;
    Hex hexUnderMouse;
    Hex hexLastUnderMouse; 

    //Camera Dragging bookkeeping vars
    public bool MouseCameraControl = true;
    private int mouseDragThreshold = 1;

    private Vector3 LastMouseGroundPlanePosition;

    delegate void UpdateFunc();
    private UpdateFunc Update_CurrentFunc;

    //Unit Movement
    private Unit __selectedUnit = null;
    public Unit SelectedUnit {
        get { return __selectedUnit; }
        set
        {
            if (SelectedCity != null) {
                SelectedCity = null;
            }

            __selectedUnit = value;
            GameManager.instance.uiUnitSelectionPanel.SetActive(__selectedUnit != null);
        }
    }

    private City __selectedCity = null;
    public City SelectedCity {
        get { return __selectedCity; }
        set
        {
            if (SelectedUnit != null) {
                SelectedUnit = null;
            }

            __selectedCity = value;
            CancelUpdateFunc();
            GameManager.instance.uiCitySelectionPanel.SetActive(__selectedCity != null);       
            Update_CurrentFunc = Update_CityView;
        }
    }


    Hex[] hexPath;
    LineRenderer lineRenderer;


    public LayerMask LayerIDForHexTiles;


    // Use this for initialization
    void Start () {
        Update_CurrentFunc = Update_DetectModeStart;
        hexMap = GameObject.FindObjectOfType<HexMap>();
        lineRenderer = transform.GetComponentInChildren<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        hexUnderMouse = MouseToHex();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            //Cancel anaything
            SelectedUnit = null;
            CancelUpdateFunc();
	    }

	    Update_CurrentFunc();

	    lastMousePosition = Input.mousePosition;
        hexLastUnderMouse = hexUnderMouse;

        if ( SelectedUnit != null && SelectedUnit.IsDestroyed ) {
            //We are pointing to a destroyed object
            SelectedUnit = null;
        }

        if ( SelectedCity != null && SelectedCity.IsDestroyed ) {
            //We are pointing to a destroyed object
            SelectedCity = null;
        }

        if ( SelectedUnit != null){
            // draw a path
            DrawPath( (hexPath != null) ? hexPath : SelectedUnit.GetHexPath()   );
        }
        else {
            DrawPath( null );  //Clear the drawn path on screen
        }

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

    Hex MouseToHex() {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        //int layerMask = 1 << (LayerIDForHexTiles);

        if (Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, LayerIDForHexTiles)) {
            //Get the parent of the object
            //var _hex = hitInfo.collider.gameObject.transform.parent.GetComponent<HexComponent>().Hex;
            GameObject hexGO = hitInfo.rigidbody.gameObject;

            //Something got hit
            //Util.WriteDebugLog(string.Format("Mouse To Hex: {0}", hitInfo.collider.gameObject.transform.parent.GetComponent<HexComponent>().Hex), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);

            return hexMap.GetHexFromGameObject(hexGO);
        }

        //Util.WriteDebugLog(string.Format("Found Nothing"), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);
        return null;
    }

    void Update_DetectModeStart() {

        //Check here if we're over a UI element and ignore if we need to ignore clicks
        if (EventSystem.current.IsPointerOverGameObject()) {
            //TODO do we want to ignore all gui objects? unit health bars, resource icons, etc
            //Although if those are set to noninteractive or not block raycasts, maybe this will return false anyway
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            //Left Mouse button is down
            //This doesn't do anything by itself, depends if we're draging etc

        }
        else if (Input.GetMouseButtonUp(0))
        {
            //Select unit
            Util.WriteDebugLog(string.Format("Mouse Click!"), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);

            Unit[] us = hexUnderMouse.Units;


            //TODO implement clicking through multiple units
            if ( us.Length > 0)
            {
                SelectedUnit = us[0];
                Util.WriteDebugLog(string.Format("Selected Unit {0}", SelectedUnit.Name), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);

                //Note selecting a unit does not change the mouse mode

                //Update_CurrentFunc = Update_UnitMovement;
            }

        }
        else if (SelectedUnit != null && Input.GetMouseButtonDown(1)){
            //We have a selected unit, and we've pushed down the right
            //mouse button
            Update_CurrentFunc = Update_UnitMovement;

        }
        // Mouse camera broke for now fix later
        //else if (Input.GetMouseButton(0) && Input.mousePosition != lastMousePosition)
        //{
        //    //todo add in a threshold to reduce folks with jittery hands
        //    //Left button is being held down and the mouse moved that's a camera drag
        //    Update_CurrentFunc = Update_CameraDrag;
        //    Update_CurrentFunc();
        //}
        else if (SelectedUnit != null && Input.GetMouseButton(1)) {
            //We have a selected unit and are holding down the mouse mbutton, show a path from 
            // unit to mouse position via the pathfinding system



        }

    }

    void Update_UnitMovement() {
        if (Input.GetMouseButtonUp(1)  || SelectedUnit == null) {
            Util.WriteDebugLog(string.Format("Complete Unit movements"), GameManager.LogLevel_Info, GameManager.instance.debug, GameManager.instance.LogLevel);

            //Todo copy pathfinding path to units movement queue
            if (SelectedUnit != null){
                SelectedUnit.SetHexPath(hexPath);

                //TODO: tell unit and or hexmap to process unit movement
                StartCoroutine(hexMap.DoUnitMoves(SelectedUnit));
            }

            Cancel();
            return;
        }



        // We have a selected unit

        // Look at hex under mouse

        // is this a different hex than before? or we don't alredy have a path
        if (hexPath == null || hexUnderMouse != hexLastUnderMouse){

            // Do a pathfinding search to that hex
            hexPath = QPath.QPath.FindPath<Hex>(
                hexMap,
                SelectedUnit,
                SelectedUnit.Hex,
                hexUnderMouse,
                Hex.CostEstimate
            );


        }


    }

    void DrawPath(Hex[] hexPath){

        if ( hexPath == null || hexPath.Length == 0){
            lineRenderer.enabled = false;
            return;
        }
        lineRenderer.enabled = true;

        Vector3[] ps = new Vector3[hexPath.Length];

        for (int h = 0; h < hexPath.Length; h++)
        {
            GameObject hexGO = hexMap.GetHexGO(hexPath[h]);
            ps[h] = hexGO.transform.position + (new Vector3(0f,0f,-0.2f));
        }

        lineRenderer.positionCount = ps.Length;
        lineRenderer.SetPositions(ps);
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

        hexPath = null;
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

    void Update_CityView() {
        //Can you still click on a unit you see during city view?
        Update_DetectModeStart();
    }



}
