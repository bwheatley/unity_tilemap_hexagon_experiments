using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QPath;
using UnityEngine;



/// <summary>
/// The hex class defines the grid position, world space position, size, neighbors, etc of a hex tile.
/// However it does NOT interact with unity directly in anyway.
/// </summary>
public class Hex : IQPathTile {

    public Hex(HexMap hexMap, int q, int r) {
        this.HexMap = hexMap;

        this.Q = q;
        this.R = r;
        this.S = -(q + r);

    }


    // Q + R + S = 0
    // S = -( Q + R )

    //TODO need a way to track property of hext type (plains, grasslands)
    //TODO need property to track hex details (forest, mine, farm)
    
    public readonly int Q;  //Column
    public readonly int R;  //Row
    public readonly int S;  //Sum

    // Data for map generation and flavor
    public float Elevation;
    public float Moisture;

    public enum TERRAIN_TYPE { PLAINS, GRASSLANDS, MARSH, FLOODPLAINS, DESERT, LAKE, OCEAN};
    public enum ELEVATION_TYPE { FLAT, HILL, MOUNTAIN, PLATEAU, SHALLOWWATER, DEEPWATER };

    public TERRAIN_TYPE TerrainType { get; set; }
    public ELEVATION_TYPE ElevationType { get; set; }

    public enum FEATURE_TYPE { NONE, FOREST, RAINFOREST, MARSH };
    public FEATURE_TYPE FeatureType { get; set; }


    static private readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    float radius = 1f;

    private HashSet<Unit> units;

    private float vertMod = 0.75f;

    public readonly HexMap HexMap;

    private Hex[] neighbors;


    public void AddUnit(Unit unit) {
        if (units == null) {
            units = new HashSet<Unit>();
        }

        units.Add(unit);
    }

    public void RemoveUnit(Unit unit) {

        if (unit != null && units != null) {
            units.Remove(unit);
        }
    }

    public Unit[] Units() {
        return units.ToArray();
    }

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

    public static float CostEstimate(IQPathTile aa, IQPathTile bb) {
        return Distance((Hex) aa, (Hex) bb);
    }

    public static float Distance(Hex a, Hex b) {

        int dQ = Mathf.Abs(a.Q - b.Q);
        if (a.HexMap.AllowWrapEastWest) {
            if (dQ > a.HexMap.numColumns / 2) {
                dQ = a.HexMap.numColumns - dQ;
            }
        }

        return Mathf.Max(
            dQ,
            Mathf.Abs(a.R - b.R),
            Mathf.Abs(a.S - b.S)
            );
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

    public Vector3 PositionFromCamera() {
        return HexMap.GetHexPosition(this);
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns) {
        //float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        if (HexMap.AllowWrapEastWest) {
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

    public override string ToString() {
        return string.Format("Q:{0}, R:{1}", this.Q, this.R);
    }

    /// <summary>
    /// Returns the most common movement cost for this tile for a typical unit 
    /// </summary>
    /// <returns>The movement cost.</returns>
    public int BaseMovementCost(bool isHillWalker, bool isForestWalker, bool isFlyer, bool isBoat, bool isMountainWalker) {
        if ((ElevationType == ELEVATION_TYPE.MOUNTAIN || ElevationType == ELEVATION_TYPE.DEEPWATER || ElevationType == ELEVATION_TYPE.SHALLOWWATER) && !isFlyer)
        {
            return -99;
        }

        int moveCost = 1;

        if (!isHillWalker && ElevationType == ELEVATION_TYPE.HILL)
        {
            moveCost++;
        }

        if ( (!isMountainWalker || !isFlyer ) && ElevationType == ELEVATION_TYPE.MOUNTAIN )
        {
            moveCost++;
        }
        if ( (!isForestWalker || !isFlyer ) && (FeatureType == FEATURE_TYPE.FOREST || FeatureType == FEATURE_TYPE.RAINFOREST) )
        {
            moveCost++;
        }

        return moveCost;
    }

    #region IQPathTile implmentation
    public IQPathTile[] GetNeighbors()
    {
        if (this.neighbors != null) {
            return this.neighbors;
        }

        //throw new System.NotImplementedException();
        List<Hex> neighbors = new List<Hex>();

        neighbors.Add(HexMap.GetHexAt(Q + 1, R + 0));
        neighbors.Add(HexMap.GetHexAt(Q - 1, R + 0));
        neighbors.Add(HexMap.GetHexAt(Q + 0, R + 1));
        neighbors.Add(HexMap.GetHexAt(Q + 0, R - 1));
        neighbors.Add(HexMap.GetHexAt(Q + 1, R - 1));
        neighbors.Add(HexMap.GetHexAt(Q - 1, R - 1));

        List<Hex> neighbors2 = new List<Hex>();

        for (int i = 0; i < neighbors.Count; i++) {
            if (neighbors[i] != null) {
                neighbors2.Add(neighbors[i]);
            }
        }

        this.neighbors = neighbors2.ToArray();
        return this.neighbors;

    }

    public float AggregateCostToEnter(float costSoFar, IQPathTile sourceTile, IQPathUnit theUnit)
    {
        //throw new System.NotImplementedException();
        //todo we are ignoring source tile right now, this will have to change when we have rivers
        return ((Unit) theUnit).AggregateTurnsToEnterHex(this, costSoFar);
    }
    #endregion
}
