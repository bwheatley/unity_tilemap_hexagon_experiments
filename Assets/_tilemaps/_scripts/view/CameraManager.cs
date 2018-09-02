using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController;
using UnityEngine.EventSystems;

public class CameraManager : CameraController.CameraController {

    public bool topdown = false;

    public override void Update()
    {
        base.Update();

        if (EventSystem.current.IsPointerOverGameObject()) {
            //MouseCameraControl = false;
            //TODO do we want to ignore all gui objects? unit health bars, resource icons, etc
            //Although if those are set to noninteractive or not block raycasts, maybe this will return false anyway
            return;
        }
        


        var rotation = new Vector3();
        if (topdown) {
            //Both do the same things
            //theCamera.transform.rotation = Quaternion.Euler(0, 0, 0);       //this is an explicit absolute set
            theCamera.transform.rotation = Quaternion.AngleAxis(0, Vector3.right);    //Also an absolute

        }
        else {
            //Both do the same things
            //theCamera.transform.rotation = Quaternion.Euler(-30, 0, 0);     //this is an explicit absolute set
            theCamera.transform.rotation = Quaternion.AngleAxis(-30, Vector3.right);
        }
    }

    


}
