using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour {

    public Vector3 oldPosition;


	// Use this for initialization
	void Start () {
	    oldPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {


	    CheckIfCameraMoved();
	}

    public void PanToHex(Hex hex) {
        //TODO move camera to hex
    }

    void CheckIfCameraMoved() {
        if (oldPosition != this.transform.position) {
            //Something moved us
            oldPosition = this.transform.position;

            //TODO make this cached etc
            //TODO eventually hexmap wil have this list for us.
            HexComponent[] hexes = GameObject.FindObjectsOfType<HexComponent>();

            for (int x = 0; x < hexes.Length; x++) {
                hexes[x].UpdatePosition();;
            }
        }
    }
}
