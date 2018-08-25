using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {

    private Vector3 newPosition;

    private Vector3 currentVelocity;
    private float smoothTime = 0.5f;


    public void OnUnitMoved(Hex oldHex, Hex newHex) {

        //Animate the unit moving from oldHex to newHex
        this.transform.position = oldHex.PositionFromCamera();
        newPosition = newHex.PositionFromCamera();
        currentVelocity = Vector3.zero;

        if (Vector3.Distance(this.transform.position, newPosition) > 2) {
            //This is a cross map move. Teleport! 
            this.transform.position = newPosition;
        }

    }

    void Start() {
        //oldPosition = newPosition = this.transform.position;
        newPosition = this.transform.position;
    }

    void Update() {

        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);


    }


}
