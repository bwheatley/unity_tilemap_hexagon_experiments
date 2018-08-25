using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraController;

public class CameraManager : CameraController.CameraController {

    public bool topdown = false;

    public override void Update()
    {
        base.Update();

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
