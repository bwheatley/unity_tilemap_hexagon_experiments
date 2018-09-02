using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitView : MonoBehaviour {

    private Vector3 newPosition;

    private Vector3 currentVelocity;
    private float smoothTime = 0.5f;
    private HexMap hexMap;

    public void OnObjectMoved(Hex oldHex, Hex newHex) {
    
        //TODO: use raycasting to determine where a unit's heights should be and remove VerticalOffset from hexmap

        //Animate the unit moving from oldHex to newHex
        Vector3 oldPosition = oldHex.PositionFromCamera();
        newPosition = newHex.PositionFromCamera();
        currentVelocity = Vector3.zero;

        //TODO: newposition.y component needs to be set from HexComponent's verticaloffset
        oldPosition.y += oldHex.HexMap.GetHexGO( oldHex ).GetComponent<HexComponent>().VerticalOffset;
        newPosition.y += newHex.HexMap.GetHexGO( newHex ).GetComponent<HexComponent>().VerticalOffset;

        this.transform.position = oldPosition;

        if ( Vector3.Distance(this.transform.position, newPosition) > 2) {
            //This is a cross map move. Teleport! 
            this.transform.position = newPosition;
        }
        else {
            //TODO we need a better signalling system and or animation queueing
            hexMap.AnimationIsPlaying = true;
        }

    }

    void Start() {
        //oldPosition = newPosition = this.transform.position;
        newPosition = this.transform.position;
        hexMap = GameObject.FindObjectOfType<HexMap>();
    }

    void Update() {

        this.transform.position = Vector3.SmoothDamp(this.transform.position, newPosition, ref currentVelocity, smoothTime);

        //TODO: figure out the best way to determine the end of our animation
        if (Vector3.Distance(this.transform.position, newPosition) < 0.1f) {
            hexMap.AnimationIsPlaying = false;
        }

    }


}
