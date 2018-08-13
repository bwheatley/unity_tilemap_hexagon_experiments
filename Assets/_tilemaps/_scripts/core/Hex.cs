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

    /// <summary>
    /// This returns the world-space position of this hex
    /// </summary>
    /// <returns></returns>
    public Vector3 Position() {

        float radius = 1f;
        float height = radius * 2;  //AKA Diameter
        float width = WIDTH_MULTIPLIER * height;

        float horiz = width;
        float vert = height * 0.75f;  // 3/4
        


        return new Vector3(horiz * (this.Q + this.R/2f)  ,
            vert * this.R,
            0);
    }


}
