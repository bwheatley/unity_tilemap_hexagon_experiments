using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexComponent : MonoBehaviour {

    public Hex Hex;
    public HexMap HexMap;

    public float VerticalOffset = 0f; //Map objects on this hex will be offset by this amount to account for look and feels

    //TODO convert this to be an ECS Component
    public void UpdatePosition() {
        this.transform.position = Hex.PositionFromCamera(Camera.main.transform.position,HexMap.numRows,HexMap.numColumns);
    }


}
