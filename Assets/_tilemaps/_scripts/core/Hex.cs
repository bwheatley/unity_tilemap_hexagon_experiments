using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The hex class defines the grid position, world space position, size, neighbors, etc of a hex tile.
/// However it does NOT interact with unity directly in anyway.
/// </summary>
public class Hex {

    public Hex(int q, int r)
    {
        this.Q = q;
        this.R = r;
        this.S = -(q + r);
    }


    // Q + R + S = 0
    // S = -( Q + R )

    public readonly int Q;  //Column
    public readonly int R;  //Row
    public readonly int S;  //Sum

    static private readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    float radius = 1f;
    private float vertMod = 0.75f;

    private bool allowWrapEastWest = true;
    //private bool allowWrapNorthSouth = false;

    /// <summary>
    /// This returns the world-space position of this hex
    /// </summary>
    /// <returns></returns>
    public Vector3 Position() {
        float vert = HexHeight() * vertMod;  // 3/4
        float horiz = HexWidth();
     
        return new Vector3(horiz * (this.Q + this.R/2f)  ,
            vert * this.R,
            0);
    }

    public float HexHeight() {
        return radius * 2;
    }

    public float HexWidth() {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing() {
        return HexHeight() * vertMod;
    }

    public float HexHorizontalSpacing() {
        return HexWidth();
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns) {
        //float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        if (allowWrapEastWest) {
            float howManyWidthsFromCamera = (position.x - cameraPosition.x) / mapWidth;

            //We want Howmanywidths from camera to be from -0.5 to 0.5
            if (Mathf.Abs(howManyWidthsFromCamera) <= 0.5f) {
                //We're good
                return position;
            }

            if (howManyWidthsFromCamera > 0) {
                howManyWidthsFromCamera += 0.5f;
            }
            else {
                howManyWidthsFromCamera -= 0.5f;
            }

            int howManyWidthToFix = (int) howManyWidthsFromCamera;

            position.x -= howManyWidthToFix * mapWidth;

        }
        return position;
    }


}
